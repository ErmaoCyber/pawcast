using Hangfire;
using Hangfire.Dashboard;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using PawCast.Api.Auth;
using PawCast.Api.Middleware;
using PawCast.Application.Abstractions;
using PawCast.Application.Services;
using PawCast.Domain.Services;
using PawCast.Infrastructure.Clients;
using PawCast.Infrastructure.Http;
using PawCast.Infrastructure.Jobs;
using PawCast.Infrastructure.Persistence;
using PawCast.Infrastructure.Persistence.Repositories;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("PawCastDb")
                      ?? throw new InvalidOperationException("Connection string 'PawCastDb' was not found.");

// Controllers
builder.Services.AddControllers();

// EF Core + PostgreSQL
builder.Services.AddDbContext<PawCastDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

// Open-Meteo options
builder.Services.Configure<OpenMeteoOptions>(
    builder.Configuration.GetSection(OpenMeteoOptions.SectionName));

// JWT options
builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection(JwtOptions.SectionName));

builder.Services.Configure<DemoUserOptions>(options =>
{
    builder.Configuration.GetSection(DemoUserOptions.SectionName).Bind(options);
});

builder.Services.AddScoped<JwtTokenService>();

// Custom validation response
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(x => x.Value?.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray());

        return new BadRequestObjectResult(new
        {
            code = "validation_error",
            message = "One or more validation errors occurred.",
            traceId = context.HttpContext.TraceIdentifier,
            errors
        });
    };
});

// Http clients
builder.Services.AddHttpClient("OpenMeteoWeather", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["OpenMeteo:WeatherBaseUrl"]!);
    client.Timeout = TimeSpan.FromSeconds(10);
})
.AddPolicyHandler((serviceProvider, _) =>
{
    var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
    var logger = loggerFactory.CreateLogger("OpenMeteoWeatherPolicy");
    return PollyPolicyFactory.CreateRetryPolicy(logger);
});

builder.Services.AddHttpClient("OpenMeteoAirQuality", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["OpenMeteo:AirQualityBaseUrl"]!);
    client.Timeout = TimeSpan.FromSeconds(10);
})
.AddPolicyHandler((serviceProvider, _) =>
{
    var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
    var logger = loggerFactory.CreateLogger("OpenMeteoAirQualityPolicy");
    return PollyPolicyFactory.CreateRetryPolicy(logger);
});

// Application / Domain / Infrastructure services
builder.Services.AddScoped<WalkIndexCalculator>();
builder.Services.AddScoped<WalkIndexQueryService>();
builder.Services.AddScoped<WalkIndexRefreshService>();

builder.Services.AddScoped<IWeatherDataProvider, OpenMeteoWeatherDataProvider>();
builder.Services.AddScoped<IWalkIndexRepository, WalkIndexRepository>();
builder.Services.AddScoped<IFetchRunRepository, FetchRunRepository>();

builder.Services.AddScoped<WalkIndexRefreshJob>();

// Hangfire
builder.Services.AddHangfire(config =>
{
    config
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UsePostgreSqlStorage(options =>
        {
            options.UseNpgsqlConnection(connectionString);
        });
});

builder.Services.AddHangfireServer();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PawCast.Api",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter 'Bearer {your JWT token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// JWT authentication
var jwtOptions = builder.Configuration
    .GetSection(JwtOptions.SectionName)
    .Get<JwtOptions>()
    ?? throw new InvalidOperationException("JWT configuration is missing.");

var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,

            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Global exception handling
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Hangfire dashboard
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new IDashboardAuthorizationFilter[]
    {
        new HangfireDashboardAuthorizationFilter()
    }
});

// Controllers
app.MapControllers();

// Recurring job: every 30 minutes
RecurringJob.AddOrUpdate<WalkIndexRefreshJob>(
    "refresh-wellington-forecast",
    job => job.RunAsync(),
    "*/30 * * * *");

app.Run();

public partial class Program { }