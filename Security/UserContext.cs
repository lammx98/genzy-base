using System.Security.Claims;
using Genzy.Base.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Genzy.Base.Security;

public interface IUserContext
{
    bool IsAuthenticated { get; }
    string? UserId { get; }
    string? Email { get; }
    /// <summary>Current workspace / organization id (JWT claim tenant_id).</summary>
    int? TenantId { get; }
    IReadOnlyList<string> Roles { get; }

    /// <summary>
    /// When true, global tenant query filters and the strict DB command guard are not applied.
    /// Use only for EF design-time tooling, explicit cross-tenant admin paths, or tests.
    /// </summary>
    bool BypassTenantQueryFilter => false;
}

public class UserContext : IUserContext
{
    public const string TenantIdClaimType = "tenant_id";

    public bool IsAuthenticated { get; set; }
    public string? UserId { get; set; }
    public string? Email { get; set; }
    public int? TenantId { get; set; }

    /// <inheritdoc />
    public bool BypassTenantQueryFilter { get; set; }

    public List<string> RolesInternal { get; } = new();
    public IReadOnlyList<string> Roles => RolesInternal;
}

public class UserContextMiddleware(UserContext context) : IMiddleware
{
    private readonly UserContext _context = context;

    public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
    {
        _context.BypassTenantQueryFilter = false;

        var principal = httpContext.User;
        _context.IsAuthenticated = principal?.Identity?.IsAuthenticated ?? false;
        if (_context.IsAuthenticated)
        {
            _context.UserId = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                               ?? principal?.FindFirst("sub")?.Value;
            _context.Email = principal?.FindFirst(ClaimTypes.Email)?.Value;
            var tenantClaim = principal?.FindFirst(UserContext.TenantIdClaimType)?.Value;
            _context.TenantId = int.TryParse(tenantClaim, out var tenantId) ? tenantId : null;
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
        services.AddScoped<TenantContextRequiredCommandInterceptor>();
        return services;
    }
}
