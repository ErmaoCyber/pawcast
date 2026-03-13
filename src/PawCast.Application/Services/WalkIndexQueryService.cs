using PawCast.Application.Abstractions;
using PawCast.Application.DTOs;
using PawCast.Domain.Services;
using PawCast.Domain.ValueObjects;

namespace PawCast.Application.Services;

public class WalkIndexQueryService
{
    private readonly IWeatherDataProvider _weatherDataProvider;
    private readonly WalkIndexCalculator _calculator;

    public WalkIndexQueryService(
        IWeatherDataProvider weatherDataProvider,
        WalkIndexCalculator calculator)
    {
        _weatherDataProvider = weatherDataProvider;
        _calculator = calculator;
    }

    public async Task<CurrentWalkIndexResponse> GetCurrentAsync(
        decimal latitude,
        decimal longitude,
        CancellationToken cancellationToken = default)
    {
        var point = await _weatherDataProvider.GetCurrentAsync(latitude, longitude, cancellationToken);

        var result = _calculator.Calculate(new WalkIndexInput(
            TemperatureC: point.TemperatureC,
            WindKph: point.WindKph,
            PrecipitationProbability: point.PrecipitationProbability,
            UvIndex: point.UvIndex,
            Pm25: point.Pm25));

        return new CurrentWalkIndexResponse(
            WalkIndex: result.Score,
            Grade: result.Grade.ToString(),
            Reasons: result.Reasons,
            Inputs: new CurrentWalkIndexInputs(
                Temperature: point.TemperatureC,
                Wind: point.WindKph,
                PrecipitationProbability: point.PrecipitationProbability,
                Uv: point.UvIndex,
                Pm25: point.Pm25),
            GeneratedAt: point.Time);
    }

    public async Task<IReadOnlyList<ForecastWalkIndexItemResponse>> GetForecastAsync(
        decimal latitude,
        decimal longitude,
        int hours,
        CancellationToken cancellationToken = default)
    {
        var points = await _weatherDataProvider.GetForecastAsync(latitude, longitude, hours, cancellationToken);

        var responses = points.Select(point =>
        {
            var result = _calculator.Calculate(new WalkIndexInput(
                TemperatureC: point.TemperatureC,
                WindKph: point.WindKph,
                PrecipitationProbability: point.PrecipitationProbability,
                UvIndex: point.UvIndex,
                Pm25: point.Pm25));

            return new ForecastWalkIndexItemResponse(
                Time: point.Time,
                WalkIndex: result.Score,
                Grade: result.Grade.ToString(),
                Reasons: result.Reasons,
                Inputs: new ForecastWalkIndexInputs(
                    Temperature: point.TemperatureC,
                    Wind: point.WindKph,
                    PrecipitationProbability: point.PrecipitationProbability,
                    Uv: point.UvIndex,
                    Pm25: point.Pm25));
        }).ToList();

        return responses;
    }
}