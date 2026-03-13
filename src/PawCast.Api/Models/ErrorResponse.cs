namespace PawCast.Api.Models;

public sealed record ErrorResponse(
    string Code,
    string Message,
    string? TraceId = null);