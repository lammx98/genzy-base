using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Genzy.Base.Security;

public interface IUserContext
{
    bool IsAuthenticated { get; }
    string? UserId { get; }
    string? Email { get; }
    /// <summary>Current workspace / organization id (JWT claim tenant_id).</summary>
    string? TenantId { get; }
    IReadOnlyList<string> Roles { get; }
}

public class UserContext : IUserContext
{
    public const string TenantIdClaimType = "tenant_id";

    public bool IsAuthenticated { get; set; }
    public string? UserId { get; set; }
    public string? Email { get; set; }
    public string? TenantId { get; set; }
    public List<string> RolesInternal { get; } = new();
    public IReadOnlyList<string> Roles => RolesInternal;
}

public class UserContextMiddleware(UserContext context) : IMiddleware
{
    private readonly UserContext _context = context;

    public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
    {
        var principal = httpContext.User;
        _context.IsAuthenticated = principal?.Identity?.IsAuthenticated ?? false;
        if (_context.IsAuthenticated)
        {
            _context.UserId = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                               ?? principal?.FindFirst("sub")?.Value;
            _context.Email = principal?.FindFirst(ClaimTypes.Email)?.Value;
            _context.TenantId = principal?.FindFirst(UserContext.TenantIdClaimType)?.Value;
            _context.RolesInternal.Clear();
            foreach (var c in principal?.FindAll(ClaimTypes.Role) ?? Array.Empty<Claim>())
            {
                _context.RolesInternal.Add(c.Value);
            }
        }
        else
        {
            _context.UserId = null;
            _context.Email = null;
            _context.TenantId = null;
            _context.RolesInternal.Clear();
        }

        await next(httpContext);
    }
}

public static class UserContextServiceCollectionExtensions
{
    public static IServiceCollection AddUserContext(this IServiceCollection services)
    {
        services.AddScoped<UserContext>();
        services.AddScoped<IUserContext>(sp => sp.GetRequiredService<UserContext>());
        services.AddScoped<UserContextMiddleware>();
        return services;
    }
}
