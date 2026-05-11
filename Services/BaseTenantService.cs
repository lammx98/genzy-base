using Genzy.Base.Exceptions;
using Genzy.Base.Models;
using Genzy.Base.Security;

namespace Genzy.Base.Services;

public abstract class BaseTenantService(IUserContext userContext)
{
    protected IUserContext UserContext => userContext;

    protected string RequireTenantId()
    {
        if (string.IsNullOrEmpty(userContext.TenantId))
            throw new UnauthorizedException("Tenant context is required.");

        return userContext.TenantId;
    }

    protected IQueryable<T> ForCurrentTenant<T>(IQueryable<T> query) where T : class, ITenantScoped
    {
        var tenantId = RequireTenantId();
        return query.Where(e => e.TenantId == tenantId);
    }
}
