using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawCast.Api.Middleware;
using PawCast.Application.Abstractions;
using PawCast.Application.Services;
using PawCast.Domain.Services;
using PawCast.Infrastructure.Clients;
using PawCast.Infrastructure.Http;
using PawCast.Infrastructure.Jobs;
using PawCast.Infrastructure.Persistence;
using PawCast.Infrastructure.Persistence.Repositories;

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
builder.Services.AddSwaggerGen();

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

app.UseAuthorization();

// Hangfire dashboard
app.UseHangfireDashboard("/hangfire");

// Controllers
app.MapControllers();

// Recurring job: every 30 minutes
RecurringJob.AddOrUpdate<WalkIndexRefreshJob>(
    "refresh-wellington-forecast",
    job => job.RunAsync(),
    "*/30 * * * *");

app.Run();

public partial class Program { }