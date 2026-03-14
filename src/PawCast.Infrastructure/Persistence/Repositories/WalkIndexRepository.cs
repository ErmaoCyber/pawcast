using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PawCast.Application.Abstractions;
using PawCast.Application.DTOs;
using PawCast.Domain.Entities;

namespace PawCast.Infrastructure.Persistence.Repositories;

public class WalkIndexRepository : IWalkIndexRepository
{
    private readonly PawCastDbContext _dbContext;

    public WalkIndexRepository(PawCastDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SaveForecastSnapshotAsync(
        decimal latitude,
        decimal longitude,
        DateTimeOffset forecastTimeUtc,
        DateTimeOffset fetchedAtUtc,
        string source,
        string rawWeatherJson,
        string rawAirQualityJson,
        CancellationToken cancellationToken = default)
    {
        var snapshot = new ForecastSnapshot(
            latitude,
            longitude,
            forecastTimeUtc,
            fetchedAtUtc,
            source,
            rawWeatherJson,
            rawAirQualityJson);

        _dbContext.ForecastSnapshots.Add(snapshot);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpsertWalkIndexResultAsync(
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
        CancellationToken cancellationToken = default)
    {
        var existing = await _dbContext.WalkIndexResults
            .FirstOrDefaultAsync(
                x => x.Latitude == latitude &&
                     x.Longitude == longitude &&
                     x.ForecastTimeUtc == forecastTimeUtc,
                cancellationToken);

        var reasonsJson = JsonSerializer.Serialize(reasons);

        if (existing is null)
        {
            var entity = new WalkIndexResultRecord(
                latitude,
                longitude,
                forecastTimeUtc,
                generatedAtUtc,
                walkIndex,
                grade,
                reasonsJson,
                temperatureC,
                windKph,
                precipitationProbability,
                uvIndex,
                pm25);

            _dbContext.WalkIndexResults.Add(entity);
        }
        else
        {
            existing.Update(
                generatedAtUtc,
                walkIndex,
                grade,
                reasonsJson,
                temperatureC,
                windKph,
                precipitationProbability,
                uvIndex,
                pm25);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<HistoryWalkIndexItemResponse>> GetHistoryAsync(
        decimal latitude,
        decimal longitude,
        int days,
        CancellationToken cancellationToken = default)
    {
        var cutoff = DateTimeOffset.UtcNow.AddDays(-days);

        var items = await _dbContext.WalkIndexResults
            .AsNoTracking()
            .Where(x =>
                x.Latitude == latitude &&
                x.Longitude == longitude &&
                x.GeneratedAtUtc >= cutoff)
            .OrderBy(x => x.ForecastTimeUtc)
            .Select(x => new
            {
                x.ForecastTimeUtc,
                x.WalkIndex,
                x.Grade,
                x.ReasonsJson,
                x.TemperatureC,
                x.WindKph,
                x.PrecipitationProbability,
                x.UvIndex,
                x.Pm25
            })
            .ToListAsync(cancellationToken);

        return items.Select(x => new HistoryWalkIndexItemResponse(
            Time: x.ForecastTimeUtc,
            WalkIndex: x.WalkIndex,
            Grade: x.Grade,
            Reasons: JsonSerializer.Deserialize<List<string>>(x.ReasonsJson) ?? new List<string>(),
            Inputs: new HistoryWalkIndexInputs(
                Temperature: x.TemperatureC,
                Wind: x.WindKph,
                PrecipitationProbability: x.PrecipitationProbability,
                Uv: x.UvIndex,
                Pm25: x.Pm25)))
            .ToList();
    }
}