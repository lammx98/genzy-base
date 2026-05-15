using Genzy.Base.Data;
using Genzy.Base.Exceptions;
using Genzy.Base.Models;
using Genzy.Base.Security;

namespace Genzy.Base.Services;

public abstract class BaseTenantService(IUserContext userContext)
{
    protected IUserContext UserContext => userContext;

    protected int RequireTenantId()
    {
        if (userContext.TenantId is null or 0)
            throw new UnauthorizedException("Tenant context is required.");

        return userContext.TenantId.Value;
    }

    protected IQueryable<T> ForCurrentTenant<T>(IQueryable<T> query) where T : class, ITenantScoped
    {
        var tenantId = RequireTenantId();
        return query.Where(e => e.TenantId == tenantId);
    }

    /// <summary>
    /// Disables EF global tenant query filters and the strict DB command guard for nested work (AsyncLocal).
    /// For a single LINQ chain, prefer <c>IgnoreQueryFilters()</c> (note: that does not disable the command guard—use this scope or <see cref="IUserContext.BypassTenantQueryFilter"/> when tenant is absent).
    /// </summary>
    protected static IDisposable WithoutTenantQueryFilter() => TenantQueryFilterScope.Suppress();
}
