namespace PawCast.Domain.ValueObjects;

public sealed record WalkIndexInput(
    decimal TemperatureC,
    decimal WindKph,
    int PrecipitationProbability,
    decimal UvIndex,
    decimal Pm25
);