using PawCast.Application.Abstractions;
using PawCast.Application.DTOs;
using PawCast.Domain.Services;
using PawCast.Domain.ValueObjects;

namespace PawCast.Application.Services;

public class WalkIndexQueryService
{
    private readonly IWeatherDataProvider _weatherDataProvider;
    private readonly WalkIndexCalculator _calculator;
    private readonly IWalkIndexRepository _repository;

    public WalkIndexQueryService(
        IWeatherDataProvider weatherDataProvider,
        WalkIndexCalculator calculator,
        IWalkIndexRepository repository)
    {
        _weatherDataProvider = weatherDataProvider;
        _calculator = calculator;
        _repository = repository;
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

        await _repository.UpsertWalkIndexResultAsync(
            latitude,
            longitude,
            point.Time,
            DateTimeOffset.UtcNow,
            result.Score,
            result.Grade.ToString(),
            result.Reasons,
            point.TemperatureC,
            point.WindKph,
            point.PrecipitationProbability,
            point.UvIndex,
            point.Pm25,
            cancellationToken);

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

        var responses = new List<ForecastWalkIndexItemResponse>();

        foreach (var point in points)
        {
            var result = _calculator.Calculate(new WalkIndexInput(
                TemperatureC: point.TemperatureC,
                WindKph: point.WindKph,
                PrecipitationProbability: point.PrecipitationProbability,
                UvIndex: point.UvIndex,
                Pm25: point.Pm25));

            await _repository.UpsertWalkIndexResultAsync(
                latitude,
                longitude,
                point.Time,
                DateTimeOffset.UtcNow,
                result.Score,
                result.Grade.ToString(),
                result.Reasons,
                point.TemperatureC,
                point.WindKph,
                point.PrecipitationProbability,
                point.UvIndex,
                point.Pm25,
                cancellationToken);

            responses.Add(new ForecastWalkIndexItemResponse(
                Time: point.Time,
                WalkIndex: result.Score,
                Grade: result.Grade.ToString(),
                Reasons: result.Reasons,
                Inputs: new ForecastWalkIndexInputs(
                    Temperature: point.TemperatureC,
                    Wind: point.WindKph,
                    PrecipitationProbability: point.PrecipitationProbability,
                    Uv: point.UvIndex,
                    Pm25: point.Pm25)));
        }

        return responses;
    }

    public Task<IReadOnlyList<HistoryWalkIndexItemResponse>> GetHistoryAsync(
    decimal latitude,
    decimal longitude,
    int days,
    CancellationToken cancellationToken = default)
    {
        return _repository.GetHistoryAsync(latitude, longitude, days, cancellationToken);
    }
}