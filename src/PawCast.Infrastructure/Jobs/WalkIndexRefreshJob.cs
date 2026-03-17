using Microsoft.Extensions.Logging;
using PawCast.Application.Services;

namespace PawCast.Infrastructure.Jobs;

public class WalkIndexRefreshJob
{
    private readonly WalkIndexRefreshService _refreshService;
    private readonly ILogger<WalkIndexRefreshJob> _logger;

    public WalkIndexRefreshJob(
        WalkIndexRefreshService refreshService,
        ILogger<WalkIndexRefreshJob> logger)
    {
        _refreshService = refreshService;
        _logger = logger;
    }

    public async Task RunAsync()
    {
        _logger.LogInformation("Starting scheduled walk index refresh job.");

        var fetchRunId = await _refreshService.RefreshWellingtonForecastAsync();

        _logger.LogInformation("Scheduled walk index refresh job completed. FetchRunId={FetchRunId}", fetchRunId);
    }
}