using PawCast.Application.DTOs;

namespace PawCast.Application.Abstractions;

public interface IWeatherDataProvider
{
    Task<WeatherForecastPoint> GetCurrentAsync(
        decimal latitude,
        decimal longitude,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<WeatherForecastPoint>> GetForecastAsync(
        decimal latitude,
        decimal longitude,
        int hours,
        CancellationToken cancellationToken = default);
}