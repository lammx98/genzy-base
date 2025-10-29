// JwtOptions.cs
namespace Genzy.Base.Security.Jwt;

public class JwtOptions
{
    public const string SectionName = "Jwt";
    // Symmetric key (base64 or raw string). For RSA you can extend this options.
    public string Key { get; set; } = null!;
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public int AccessTokenExpirationMinutes { get; set; } = 15;
    public int RefreshTokenExpirationDays { get; set; } = 30;
}
