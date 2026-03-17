namespace PawCast.Api.Auth;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "PawCast";
    public string Audience { get; set; } = "PawCast.Client";
    public string SigningKey { get; set; } = "CHANGE_THIS_TO_A_LONG_DEV_SECRET_KEY_12345";
    public int ExpiryMinutes { get; set; } = 120;
}