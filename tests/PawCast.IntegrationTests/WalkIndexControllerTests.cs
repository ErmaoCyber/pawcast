using System.Net;
using System.Net.Http.Json;

namespace PawCast.IntegrationTests;

public class WalkIndexControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public WalkIndexControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetCurrent_ShouldReturnOk_WhenRequestIsValid()
    {
        var response = await _client.GetAsync("/api/walkindex/current?lat=-41.2865&lon=174.7762");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<CurrentResponseTestDto>();
        Assert.NotNull(payload);
        Assert.Equal(100, payload!.WalkIndex);
        Assert.Equal("Excellent", payload.Grade);
    }

    [Fact]
    public async Task GetForecast_ShouldReturnOk_WhenRequestIsValid()
    {
        var response = await _client.GetAsync("/api/walkindex/forecast?lat=-41.2865&lon=174.7762&hours=6");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<List<ForecastResponseTestDto>>();
        Assert.NotNull(payload);
        Assert.Equal(6, payload!.Count);
        Assert.All(payload, item => Assert.Equal("Excellent", item.Grade));
    }

    [Fact]
    public async Task GetCurrent_ShouldReturnBadRequest_WhenLatitudeIsOutOfRange()
    {
        var response = await _client.GetAsync("/api/walkindex/current?lat=-200&lon=174.7762");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetForecast_ShouldReturnBadRequest_WhenHoursIsOutOfRange()
    {
        var response = await _client.GetAsync("/api/walkindex/forecast?lat=-41.2865&lon=174.7762&hours=100");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private sealed class CurrentResponseTestDto
    {
        public int WalkIndex { get; set; }
        public string Grade { get; set; } = string.Empty;
    }

    private sealed class ForecastResponseTestDto
    {
        public string Grade { get; set; } = string.Empty;
    }
}