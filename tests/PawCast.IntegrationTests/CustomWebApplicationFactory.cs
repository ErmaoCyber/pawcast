using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PawCast.Application.Abstractions;
using PawCast.Application.DTOs;

namespace PawCast.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IWeatherDataProvider>();
            services.AddScoped<IWeatherDataProvider, FakeWeatherDataProvider>();
        });
    }
}

public class FakeWeatherDataProvider : IWeatherDataProvider
{
    public Task<WeatherForecastPoint> GetCurrentAsync(
        decimal latitude,
        decimal longitude,
        CancellationToken cancellationToken = default)
    {
        var point = new WeatherForecastPoint(
            Time: DateTimeOffset.Parse("2026-03-13T08:00:00Z"),
            TemperatureC: 18,
            WindKph: 10,
            PrecipitationProbability: 5,
            UvIndex: 2,
            Pm25: 5);

        return Task.FromResult(point);
    }

    public Task<IReadOnlyList<WeatherForecastPoint>> GetForecastAsync(
        decimal latitude,
        decimal longitude,
        int hours,
        CancellationToken cancellationToken = default)
    {
        var list = Enumerable.Range(0, hours)
            .Select(i => new WeatherForecastPoint(
                Time: DateTimeOffset.Parse("2026-03-13T08:00:00Z").AddHours(i),
                TemperatureC: 18,
                WindKph: 10,
                PrecipitationProbability: 5,
                UvIndex: 2,
                Pm25: 5))
            .ToList();

        return Task.FromResult<IReadOnlyList<WeatherForecastPoint>>(list);
    }
}