using System.Security.Claims;

namespace GCL.Base.Security.Jwt;

public static class ClaimHelpers
{
    public static string? GetUserId(this ClaimsPrincipal principal)
        => principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    public static string? GetEmail(this ClaimsPrincipal principal)
        => principal?.FindFirst(ClaimTypes.Email)?.Value;
}
