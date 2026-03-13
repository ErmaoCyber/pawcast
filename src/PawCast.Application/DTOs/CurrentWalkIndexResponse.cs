namespace PawCast.Application.DTOs;

public sealed record CurrentWalkIndexResponse(
    int WalkIndex,
    string Grade,
    IReadOnlyList<string> Reasons,
    CurrentWalkIndexInputs Inputs,
    DateTimeOffset GeneratedAt);

public sealed record CurrentWalkIndexInputs(
    decimal Temperature,
    decimal Wind,
    int PrecipitationProbability,
    decimal Uv,
    decimal Pm25);