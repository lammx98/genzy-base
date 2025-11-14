// TokenService.cs
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Genzy.Base.Security.Jwt;

public interface IJwtService
{
    JwtResult CreateTokens(IEnumerable<Claim> claims, string? userId = null);
    ClaimsPrincipal? ValidateToken(string token, bool validateLifetime = true);
}
public class JwtService : IJwtService
{
    private readonly JwtOptions _opts;
    private readonly JwtSecurityTokenHandler _handler = new JwtSecurityTokenHandler();

    public JwtService(JwtOptions opts)
    {
        _opts = opts;
    }

    public JwtResult CreateTokens(IEnumerable<Claim> claims, string? userId = null)
    {
        var now = DateTime.UtcNow;
        var keyBytes = Encoding.UTF8.GetBytes(_opts.Secret);
        var signingKey = new SymmetricSecurityKey(keyBytes);
        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var jwtClaims = claims.ToList();
        if (!string.IsNullOrEmpty(userId) && !jwtClaims.Any(c => c.Type == ClaimTypes.NameIdentifier))
            jwtClaims.Add(new Claim(ClaimTypes.NameIdentifier, userId));

        var accessExp = now.AddMinutes(_opts.ExpiryMinutes);
        var jwt = new JwtSecurityToken(
            issuer: _opts.Issuer,
            audience: _opts.Audience,
            claims: jwtClaims,
            notBefore: now,
            expires: accessExp,
            signingCredentials: creds
        );

        var accessToken = _handler.WriteToken(jwt);

        // refresh token - here simple random string, store hashed in DB/Redis
        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var refreshExp = now.AddDays(_opts.RefreshTokenExpiryDays);

        return new JwtResult
        {
            AccessToken = accessToken,
            AccessTokenExpiresAt = accessExp,
            RefreshToken = refreshToken,
            RefreshTokenExpiresAt = refreshExp
        };
    }

    public ClaimsPrincipal? ValidateToken(string token, bool validateLifetime = true)
    {
        var keyBytes = Encoding.UTF8.GetBytes(_opts.Secret);
        var signingKey = new SymmetricSecurityKey(keyBytes);
        var parameters = new TokenValidationParameters
        {
            ValidateIssuer = !string.IsNullOrEmpty(_opts.Issuer),
            ValidateAudience = !string.IsNullOrEmpty(_opts.Audience),
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            ValidateLifetime = validateLifetime,
            ClockSkew = TimeSpan.FromSeconds(60),
            ValidIssuer = _opts.Issuer,
            ValidAudience = _opts.Audience
        };

        try
        {
            var principal = _handler.ValidateToken(token, parameters, out var validatedToken);
            // optionally check algorithm/signature details
            return principal;
        }
        catch
        {
            return null;
        }
    }
}
