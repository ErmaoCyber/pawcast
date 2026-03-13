namespace PawCast.Application.DTOs;

public sealed record WeatherForecastPoint(
    DateTimeOffset Time,
    decimal TemperatureC,
    decimal WindKph,
    int PrecipitationProbability,
    decimal UvIndex,
    decimal Pm25
);