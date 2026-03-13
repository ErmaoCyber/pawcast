using Microsoft.AspNetCore.Mvc;
using PawCast.Api.Models;
using PawCast.Application.Services;

namespace PawCast.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WalkIndexController : ControllerBase
{
    private readonly WalkIndexQueryService _service;

    public WalkIndexController(WalkIndexQueryService service)
    {
        _service = service;
    }

    [HttpGet("current")]
    public async Task<IActionResult> GetCurrent(
        [FromQuery] WalkIndexCurrentQueryRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetCurrentAsync(
            request.Lat,
            request.Lon,
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("forecast")]
    public async Task<IActionResult> GetForecast(
        [FromQuery] WalkIndexForecastQueryRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await _service.GetForecastAsync(
            request.Lat,
            request.Lon,
            request.Hours,
            cancellationToken);

        return Ok(result);
    }
}