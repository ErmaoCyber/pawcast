using Hangfire;
using Microsoft.AspNetCore.Mvc;
using PawCast.Application.Abstractions;
using PawCast.Infrastructure.Jobs;

namespace PawCast.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OpsController : ControllerBase
{
    private readonly IFetchRunRepository _fetchRunRepository;

    public OpsController(IFetchRunRepository fetchRunRepository)
    {
        _fetchRunRepository = fetchRunRepository;
    }

    [HttpPost("refresh")]
    public IActionResult TriggerRefresh()
    {
        var jobId = BackgroundJob.Enqueue<WalkIndexRefreshJob>(job => job.RunAsync());

        return Accepted(new
        {
            message = "Refresh job has been queued.",
            jobId
        });
    }

    [HttpGet("fetch-runs")]
    public async Task<IActionResult> GetFetchRuns(
        [FromQuery] int count = 20,
        CancellationToken cancellationToken = default)
    {
        if (count <= 0 || count > 100)
        {
            return BadRequest("count must be between 1 and 100.");
        }

        var items = await _fetchRunRepository.GetRecentAsync(count, cancellationToken);
        return Ok(items);
    }
}