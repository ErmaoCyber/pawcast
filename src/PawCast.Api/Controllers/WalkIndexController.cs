using Microsoft.AspNetCore.Mvc;
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
        [FromQuery] decimal lat,
        [FromQuery] decimal lon,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetCurrentAsync(lat, lon, cancellationToken);
        return Ok(result);
    }

    [HttpGet("forecast")]
    public async Task<IActionResult> GetForecast(
        [FromQuery] decimal lat,
        [FromQuery] decimal lon,
        [FromQuery] int hours = 24,
        CancellationToken cancellationToken = default)
    {
        if (hours <= 0 || hours > 72)
        {
            return BadRequest("hours must be between 1 and 72.");
        }

        var result = await _service.GetForecastAsync(lat, lon, hours, cancellationToken);
        return Ok(result);
    }
}