using System.Text.Json.Serialization;

namespace PawCast.Infrastructure.Clients;

public sealed class WeatherForecastApiResponse
{
    [JsonPropertyName("hourly")]
    public WeatherHourlyData? Hourly { get; set; }

    public sealed class WeatherHourlyData
    {
        [JsonPropertyName("time")]
        public List<string> Time { get; set; } = new();

        [JsonPropertyName("temperature_2m")]
        public List<decimal?> Temperature2m { get; set; } = new();

        [JsonPropertyName("wind_speed_10m")]
        public List<decimal?> WindSpeed10m { get; set; } = new();

        [JsonPropertyName("precipitation_probability")]
        public List<int?> PrecipitationProbability { get; set; } = new();
    }
}