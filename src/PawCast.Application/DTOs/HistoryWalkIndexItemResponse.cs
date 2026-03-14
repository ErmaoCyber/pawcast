namespace PawCast.Application.DTOs;

public sealed record HistoryWalkIndexItemResponse(
    DateTimeOffset Time,
    int WalkIndex,
    string Grade,
    IReadOnlyList<string> Reasons,
    HistoryWalkIndexInputs Inputs);

public sealed record HistoryWalkIndexInputs(
    decimal Temperature,
    decimal Wind,
    int PrecipitationProbability,
    decimal Uv,
    decimal Pm25);