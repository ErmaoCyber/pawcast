using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PawCast.Application.Abstractions;
using PawCast.Application.DTOs;

namespace PawCast.Infrastructure.Clients;

public class OpenMeteoWeatherDataProvider : IWeatherDataProvider
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly OpenMeteoOptions _options;
    private readonly ILogger<OpenMeteoWeatherDataProvider> _logger;

    public OpenMeteoWeatherDataProvider(
        IHttpClientFactory httpClientFactory,
        IOptions<OpenMeteoOptions> options,
        ILogger<OpenMeteoWeatherDataProvider> logger)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<WeatherForecastPoint> GetCurrentAsync(
        decimal latitude,
        decimal longitude,
        CancellationToken cancellationToken = default)
    {
        var forecast = await GetForecastAsync(latitude, longitude, 1, cancellationToken);
        return forecast[0];
    }

    public async Task<IReadOnlyList<WeatherForecastPoint>> GetForecastAsync(
        decimal latitude,
        decimal longitude,
        int hours,
        CancellationToken cancellationToken = default)
    {
        if (hours <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(hours), "Hours must be greater than zero.");
        }

        _logger.LogInformation(
            "Fetching Open-Meteo forecast data. Latitude={Latitude}, Longitude={Longitude}, Hours={Hours}",
            latitude,
            longitude,
            hours);

        var weatherTask = GetWeatherAsync(latitude, longitude, hours, cancellationToken);
        var airQualityTask = GetAirQualityAsync(latitude, longitude, hours, cancellationToken);

        await Task.WhenAll(weatherTask, airQualityTask);

        var merged = MergeHourlyData(
            weatherTask.Result,
            airQualityTask.Result,
            hours);

        _logger.LogInformation(
            "Successfully merged Open-Meteo forecast data. Latitude={Latitude}, Longitude={Longitude}, Hours={Hours}, Points={Points}",
            latitude,
            longitude,
            hours,
            merged.Count);

        return merged;
    }

    private async Task<WeatherForecastApiResponse> GetWeatherAsync(
        decimal latitude,
        decimal longitude,
        int hours,
        CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient("OpenMeteoWeather");

        var url =
            $"v1/forecast?latitude={latitude}&longitude={longitude}" +
            $"&hourly=temperature_2m,wind_speed_10m,precipitation_probability" +
            $"&forecast_hours={hours}&timezone=GMT";

        _logger.LogInformation(
            "Requesting weather forecast from Open-Meteo. Url={Url}",
            url);

        using var response = await client.GetAsync(url, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            _logger.LogError(
                "Weather API request failed. StatusCode={StatusCode}, Body={Body}",
                (int)response.StatusCode,
                body);

            throw new HttpRequestException(
                $"Weather API request failed with status code {(int)response.StatusCode}.");
        }

        var payload = await response.Content.ReadFromJsonAsync<WeatherForecastApiResponse>(
                          cancellationToken: cancellationToken)
                      ?? throw new InvalidOperationException("Weather API returned empty payload.");

        if (payload.Hourly is null)
        {
            throw new InvalidOperationException("Weather API payload missing hourly data.");
        }

        return payload;
    }

    private async Task<AirQualityApiResponse> GetAirQualityAsync(
        decimal latitude,
        decimal longitude,
        int hours,
        CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient("OpenMeteoAirQuality");

        var url =
            $"v1/air-quality?latitude={latitude}&longitude={longitude}" +
            $"&hourly=pm2_5,uv_index" +
            $"&forecast_hours={hours}&timezone=GMT";

        _logger.LogInformation(
            "Requesting air quality forecast from Open-Meteo. Url={Url}",
            url);

        using var response = await client.GetAsync(url, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            _logger.LogError(
                "Air quality API request failed. StatusCode={StatusCode}, Body={Body}",
                (int)response.StatusCode,
                body);

            throw new HttpRequestException(
                $"Air quality API request failed with status code {(int)response.StatusCode}.");
        }

        var payload = await response.Content.ReadFromJsonAsync<AirQualityApiResponse>(
                          cancellationToken: cancellationToken)
                      ?? throw new InvalidOperationException("Air quality API returned empty payload.");

        if (payload.Hourly is null)
        {
            throw new InvalidOperationException("Air quality API payload missing hourly data.");
        }

        return payload;
    }

    private static IReadOnlyList<WeatherForecastPoint> MergeHourlyData(
        WeatherForecastApiResponse weather,
        AirQualityApiResponse airQuality,
        int hours)
    {
        var weatherHourly = weather.Hourly!;
        var airHourly = airQuality.Hourly!;

        var weatherMap = Enumerable.Range(0, weatherHourly.Time.Count)
            .ToDictionary(
                i => weatherHourly.Time[i],
                i => new
                {
                    Temperature = weatherHourly.Temperature2m.ElementAtOrDefault(i),
                    Wind = weatherHourly.WindSpeed10m.ElementAtOrDefault(i),
                    PrecipitationProbability = weatherHourly.PrecipitationProbability.ElementAtOrDefault(i)
                });

        var airMap = Enumerable.Range(0, airHourly.Time.Count)
            .ToDictionary(
                i => airHourly.Time[i],
                i => new
                {
                    Pm25 = airHourly.Pm25.ElementAtOrDefault(i),
                    Uv = airHourly.UvIndex.ElementAtOrDefault(i)
                });

        var merged = new List<WeatherForecastPoint>();

        foreach (var time in weatherHourly.Time.Take(hours))
        {
            if (!weatherMap.TryGetValue(time, out var weatherItem))
            {
                continue;
            }

            if (!airMap.TryGetValue(time, out var airItem))
            {
                continue;
            }

            if (weatherItem.Temperature is null ||
                weatherItem.Wind is null ||
                weatherItem.PrecipitationProbability is null ||
                airItem.Pm25 is null ||
                airItem.Uv is null)
            {
                continue;
            }

            merged.Add(new WeatherForecastPoint(
                Time: DateTimeOffset.Parse(time).ToUniversalTime(),
                TemperatureC: weatherItem.Temperature.Value,
                WindKph: weatherItem.Wind.Value,
                PrecipitationProbability: weatherItem.PrecipitationProbability.Value,
                UvIndex: airItem.Uv.Value,
                Pm25: airItem.Pm25.Value));
        }

        if (merged.Count == 0)
        {
            throw new InvalidOperationException("No overlapping forecast data could be merged from Open-Meteo responses.");
        }

        return merged;
    }
}