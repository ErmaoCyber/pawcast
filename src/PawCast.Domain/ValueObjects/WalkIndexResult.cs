using PawCast.Domain.Enums;

namespace PawCast.Domain.ValueObjects;

public sealed record WalkIndexResult(
    int Score,
    WalkIndexGrade Grade,
    IReadOnlyList<string> Reasons
);