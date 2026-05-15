namespace Genzy.Base.Data;

/// <summary>
/// Supplies values evaluated at query time for <see cref="TenantScopedQueryFilterExtensions"/>.
/// <see cref="IsTenantQueryFilterSuppressed"/> includes <see cref="TenantQueryFilterScope"/> and <see cref="Genzy.Base.Security.IUserContext.BypassTenantQueryFilter"/>.
/// </summary>
public interface ITenantGlobalQueryFilterContext
{
    bool IsTenantQueryFilterSuppressed { get; }

    /// <summary>Current tenant id; when null and filters are not suppressed, no tenant-scoped rows match.</summary>
    int? TenantIdForGlobalQueryFilter { get; }
}
