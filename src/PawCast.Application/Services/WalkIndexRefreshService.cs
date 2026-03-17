using PawCast.Application.Abstractions;

namespace PawCast.Application.Services;

public class WalkIndexRefreshService
{
    private readonly WalkIndexQueryService _queryService;
    private readonly IFetchRunRepository _fetchRunRepository;

    public WalkIndexRefreshService(
        WalkIndexQueryService queryService,
        IFetchRunRepository fetchRunRepository)
    {
        _queryService = queryService;
        _fetchRunRepository = fetchRunRepository;
    }

    public async Task<Guid> RefreshWellingtonForecastAsync(
        CancellationToken cancellationToken = default)
    {
        var startedAtUtc = DateTimeOffset.UtcNow;
        var fetchRunId = await _fetchRunRepository.CreateStartedAsync(startedAtUtc, cancellationToken);

        try
        {
            await _queryService.GetForecastAsync(
                latitude: -41.2865m,
                longitude: 174.7762m,
                hours: 24,
                cancellationToken: cancellationToken);

            await _fetchRunRepository.MarkSucceededAsync(
                fetchRunId,
                DateTimeOffset.UtcNow,
                cancellationToken);

            return fetchRunId;
        }
        catch (Exception ex)
        {
            await _fetchRunRepository.MarkFailedAsync(
                fetchRunId,
                DateTimeOffset.UtcNow,
                ex.Message,
                cancellationToken);

            throw;
        }
    }
}