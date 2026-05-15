namespace Genzy.Base.Models;

/// <summary>
/// Row belongs to a single tenant (shared database, tenant column).
/// With <see cref="Genzy.Base.Data.AuditableDbContext{T}"/>, reads are scoped by a global query filter (no match when tenant is missing unless suppressed)
/// and inserts get <see cref="TenantId"/> on save when unset (0).
/// Opt out per entity type with <see cref="SkipTenantQueryFilterAttribute"/>, per query with <c>IgnoreQueryFilters()</c>,
/// or globally for the current flow with <see cref="TenantQueryFilterScope"/> / <see cref="Genzy.Base.Security.IUserContext.BypassTenantQueryFilter"/>.
/// </summary>
public interface ITenantScoped
{
    int TenantId { get; set; }
}
