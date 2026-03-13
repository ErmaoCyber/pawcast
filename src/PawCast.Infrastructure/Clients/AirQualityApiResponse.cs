using System.Text.Json.Serialization;

namespace PawCast.Infrastructure.Clients;

public sealed class AirQualityApiResponse
{
    [JsonPropertyName("hourly")]
    public AirQualityHourlyData? Hourly { get; set; }

    public sealed class AirQualityHourlyData
    {
        [JsonPropertyName("time")]
        public List<string> Time { get; set; } = new();

        [JsonPropertyName("pm2_5")]
        public List<decimal?> Pm25 { get; set; } = new();

        [JsonPropertyName("uv_index")]
        public List<decimal?> UvIndex { get; set; } = new();
    }
}