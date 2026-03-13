using Microsoft.EntityFrameworkCore;
using PawCast.Application.Abstractions;
using PawCast.Application.Services;
using PawCast.Domain.Services;
using PawCast.Infrastructure.Clients;
using PawCast.Infrastructure.Persistence;
using PawCast.Api.Middleware;
using Microsoft.AspNetCore.Mvc;
using PawCast.Api.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<PawCastDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("PawCastDb")
                          ?? throw new InvalidOperationException("Connection string 'PawCastDb' was not found.");

    options.UseNpgsql(connectionString);
});

builder.Services.Configure<OpenMeteoOptions>(
    builder.Configuration.GetSection(OpenMeteoOptions.SectionName));

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

builder.Services.AddHttpClient("OpenMeteoWeather", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["OpenMeteo:WeatherBaseUrl"]!);
    client.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddHttpClient("OpenMeteoAirQuality", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["OpenMeteo:AirQualityBaseUrl"]!);
    client.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddScoped<WalkIndexCalculator>();
builder.Services.AddScoped<WalkIndexQueryService>();
builder.Services.AddScoped<IWeatherDataProvider, OpenMeteoWeatherDataProvider>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
public partial class Program { }