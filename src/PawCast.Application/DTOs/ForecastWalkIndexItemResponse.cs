namespace PawCast.Application.DTOs;

public sealed record ForecastWalkIndexItemResponse(
    DateTimeOffset Time,
    int WalkIndex,
    string Grade,
    IReadOnlyList<string> Reasons,
    ForecastWalkIndexInputs Inputs);

public sealed record ForecastWalkIndexInputs(
    decimal Temperature,
    decimal Wind,
    int PrecipitationProbability,
    decimal Uv,
    decimal Pm25);