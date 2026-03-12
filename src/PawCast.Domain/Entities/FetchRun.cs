namespace PawCast.Domain.Entities;

public class FetchRun
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public DateTimeOffset StartedAtUtc { get; private set; }
    public DateTimeOffset? EndedAtUtc { get; private set; }
    public string Status { get; private set; } = default!;
    public string? ErrorMessage { get; private set; }
    public long? DurationMs { get; private set; }

    private FetchRun() { } // EF Core

    public FetchRun(DateTimeOffset startedAtUtc, string status)
    {
        StartedAtUtc = startedAtUtc;
        Status = status;
    }

    public void MarkSucceeded(DateTimeOffset endedAtUtc)
    {
        EndedAtUtc = endedAtUtc;
        DurationMs = (long)(EndedAtUtc.Value - StartedAtUtc).TotalMilliseconds;
        Status = "Succeeded";
        ErrorMessage = null;
    }

    public void MarkFailed(DateTimeOffset endedAtUtc, string errorMessage)
    {
        EndedAtUtc = endedAtUtc;
        DurationMs = (long)(EndedAtUtc.Value - StartedAtUtc).TotalMilliseconds;
        Status = "Failed";
        ErrorMessage = errorMessage;
    }
}