namespace PawCast.Infrastructure.Clients;

public sealed class OpenMeteoOptions
{
    public const string SectionName = "OpenMeteo";

    public string WeatherBaseUrl { get; set; } = "https://api.open-meteo.com/";
    public string AirQualityBaseUrl { get; set; } = "https://air-quality-api.open-meteo.com/";
}