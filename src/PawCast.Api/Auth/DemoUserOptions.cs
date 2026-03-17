namespace PawCast.Api.Auth;

public sealed class DemoUserOptions
{
    public const string SectionName = "DemoUsers";

    public List<DemoUser> Users { get; set; } = new();
}