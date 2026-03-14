using PawCast.Application.DTOs;

namespace PawCast.Application.Abstractions;

public interface IWalkIndexRepository
{
    Task SaveForecastSnapshotAsync(
        decimal latitude,
        decimal longitude,
        DateTimeOffset forecastTimeUtc,
        DateTimeOffset fetchedAtUtc,
        string source,
        string rawWeatherJson,
        string rawAirQualityJson,
        CancellationToken cancellationToken = default);

    Task UpsertWalkIndexResultAsync(
        decimal latitude,
        decimal longitude,
        DateTimeOffset forecastTimeUtc,
        DateTimeOffset generatedAtUtc,
        int walkIndex,
        string grade,
        IReadOnlyList<string> reasons,
        decimal temperatureC,
        decimal windKph,
        int precipitationProbability,
        decimal uvIndex,
        decimal pm25,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<HistoryWalkIndexItemResponse>> GetHistoryAsync(
        decimal latitude,
        decimal longitude,
        int days,
        CancellationToken cancellationToken = default);
}