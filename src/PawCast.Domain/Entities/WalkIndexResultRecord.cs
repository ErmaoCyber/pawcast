namespace PawCast.Domain.Entities;

public class WalkIndexResultRecord
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public decimal Latitude { get; private set; }
    public decimal Longitude { get; private set; }

    public DateTimeOffset ForecastTimeUtc { get; private set; }
    public DateTimeOffset GeneratedAtUtc { get; private set; }

    public int WalkIndex { get; private set; }
    public string Grade { get; private set; } = default!;
    public string ReasonsJson { get; private set; } = default!;

    public decimal TemperatureC { get; private set; }
    public decimal WindKph { get; private set; }
    public int PrecipitationProbability { get; private set; }
    public decimal UvIndex { get; private set; }
    public decimal Pm25 { get; private set; }

    private WalkIndexResultRecord() { }

    public WalkIndexResultRecord(
        decimal latitude,
        decimal longitude,
        DateTimeOffset forecastTimeUtc,
        DateTimeOffset generatedAtUtc,
        int walkIndex,
        string grade,
        string reasonsJson,
        decimal temperatureC,
        decimal windKph,
        int precipitationProbability,
        decimal uvIndex,
        decimal pm25)
    {
        Latitude = latitude;
        Longitude = longitude;
        ForecastTimeUtc = forecastTimeUtc;
        GeneratedAtUtc = generatedAtUtc;
        WalkIndex = walkIndex;
        Grade = grade;
        ReasonsJson = reasonsJson;
        TemperatureC = temperatureC;
        WindKph = windKph;
        PrecipitationProbability = precipitationProbability;
        UvIndex = uvIndex;
        Pm25 = pm25;
    }

    public void Update(
        DateTimeOffset generatedAtUtc,
        int walkIndex,
        string grade,
        string reasonsJson,
        decimal temperatureC,
        decimal windKph,
        int precipitationProbability,
        decimal uvIndex,
        decimal pm25)
    {
        GeneratedAtUtc = generatedAtUtc;
        WalkIndex = walkIndex;
        Grade = grade;
        ReasonsJson = reasonsJson;
        TemperatureC = temperatureC;
        WindKph = windKph;
        PrecipitationProbability = precipitationProbability;
        UvIndex = uvIndex;
        Pm25 = pm25;
    }
}