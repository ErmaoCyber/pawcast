using Microsoft.EntityFrameworkCore;
using PawCast.Application.Abstractions;

namespace PawCast.Infrastructure.Persistence.Repositories;

public class FetchRunRepository : IFetchRunRepository
{
    private readonly PawCastDbContext _dbContext;

    public FetchRunRepository(PawCastDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid> CreateStartedAsync(
        DateTimeOffset startedAtUtc,
        CancellationToken cancellationToken = default)
    {
        startedAtUtc = startedAtUtc.ToUniversalTime();

        var entity = new Domain.Entities.FetchRun(startedAtUtc, "Started");

        _dbContext.FetchRuns.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    public async Task MarkSucceededAsync(
        Guid fetchRunId,
        DateTimeOffset endedAtUtc,
        CancellationToken cancellationToken = default)
    {
        endedAtUtc = endedAtUtc.ToUniversalTime();

        var entity = await _dbContext.FetchRuns
            .FirstOrDefaultAsync(x => x.Id == fetchRunId, cancellationToken)
            ?? throw new InvalidOperationException($"FetchRun {fetchRunId} was not found.");

        entity.MarkSucceeded(endedAtUtc);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task MarkFailedAsync(
        Guid fetchRunId,
        DateTimeOffset endedAtUtc,
        string errorMessage,
        CancellationToken cancellationToken = default)
    {
        endedAtUtc = endedAtUtc.ToUniversalTime();

        var entity = await _dbContext.FetchRuns
            .FirstOrDefaultAsync(x => x.Id == fetchRunId, cancellationToken)
            ?? throw new InvalidOperationException($"FetchRun {fetchRunId} was not found.");

        entity.MarkFailed(endedAtUtc, errorMessage);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<FetchRunDto>> GetRecentAsync(
        int count,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.FetchRuns
            .AsNoTracking()
            .OrderByDescending(x => x.StartedAtUtc)
            .Take(count)
            .Select(x => new FetchRunDto(
                x.Id,
                x.StartedAtUtc,
                x.EndedAtUtc,
                x.Status,
                x.ErrorMessage,
                x.DurationMs))
            .ToListAsync(cancellationToken);
    }
}