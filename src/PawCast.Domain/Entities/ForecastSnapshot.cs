namespace PawCast.Domain.Entities;

public class ForecastSnapshot
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public decimal Latitude { get; private set; }
    public decimal Longitude { get; private set; }

    public DateTimeOffset ForecastTimeUtc { get; private set; }
    public DateTimeOffset FetchedAtUtc { get; private set; }

    public string Source { get; private set; } = default!;

    public string RawWeatherJson { get; private set; } = default!;
    public string RawAirQualityJson { get; private set; } = default!;

    private ForecastSnapshot() { }

    public ForecastSnapshot(
        decimal latitude,
        decimal longitude,
        DateTimeOffset forecastTimeUtc,
        DateTimeOffset fetchedAtUtc,
        string source,
        string rawWeatherJson,
        string rawAirQualityJson)
    {
        Latitude = latitude;
        Longitude = longitude;
        ForecastTimeUtc = forecastTimeUtc;
        FetchedAtUtc = fetchedAtUtc;
        Source = source;
        RawWeatherJson = rawWeatherJson;
        RawAirQualityJson = rawAirQualityJson;
    }
}