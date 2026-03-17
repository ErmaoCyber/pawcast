using Microsoft.AspNetCore.Mvc;
using PawCast.Api.Models;
using PawCast.Application.Services;
using Microsoft.AspNetCore.Authorization;

namespace PawCast.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "user,ops")]
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

    [HttpGet("history")]
    public async Task<IActionResult> GetHistory(
    [FromQuery] WalkIndexHistoryQueryRequest request,
    CancellationToken cancellationToken = default)
    {
        var result = await _service.GetHistoryAsync(
            request.Lat,
            request.Lon,
            request.Days,
            cancellationToken);

        return Ok(result);
    }
}