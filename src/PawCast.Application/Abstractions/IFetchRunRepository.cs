namespace PawCast.Application.Abstractions;

public interface IFetchRunRepository
{
    Task<Guid> CreateStartedAsync(
        DateTimeOffset startedAtUtc,
        CancellationToken cancellationToken = default);

    Task MarkSucceededAsync(
        Guid fetchRunId,
        DateTimeOffset endedAtUtc,
        CancellationToken cancellationToken = default);

    Task MarkFailedAsync(
        Guid fetchRunId,
        DateTimeOffset endedAtUtc,
        string errorMessage,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<FetchRunDto>> GetRecentAsync(
        int count,
        CancellationToken cancellationToken = default);
}

public sealed record FetchRunDto(
    Guid Id,
    DateTimeOffset StartedAtUtc,
    DateTimeOffset? EndedAtUtc,
    string Status,
    string? ErrorMessage,
    long? DurationMs);