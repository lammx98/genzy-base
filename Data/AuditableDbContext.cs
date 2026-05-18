using Genzy.Base.Models;
using Genzy.Base.Security;
using Microsoft.EntityFrameworkCore;

namespace Genzy.Base.Data;

/// <summary>
/// <see cref="BaseDbContext{T}"/> that automatically applies <see cref="DbContextAuditExtensions.ApplyAutomaticAuditFields"/> on save.
/// For contexts that should not auto-audit, use <see cref="BaseDbContext{T}"/> and optionally call the extension manually.
/// </summary>
public abstract class AuditableDbContext<T>(
    DbContextOptions<T> options,
    IUserContext? userContext = null) : BaseDbContext<T>(options), ITenantGlobalQueryFilterContext
    where T : DbContext
{
    /// <summary>User context used for <see cref="BaseAudit2"/> actor fields; may be null (e.g. design-time, background jobs).</summary>
    protected IUserContext? AuditUserContext => userContext;

    private bool IsTenantQueryFilterSuppressedCore =>
        TenantQueryFilterScope.IsSuppressed || userContext?.BypassTenantQueryFilter == true;

    bool ITenantGlobalQueryFilterContext.IsTenantQueryFilterSuppressed => IsTenantQueryFilterSuppressedCore;

    int? ITenantGlobalQueryFilterContext.TenantIdForGlobalQueryFilter => userContext?.TenantId;

    /// <summary>
    /// When true, global tenant filters are not applied (see <see cref="TenantQueryFilterScope"/> or <see cref="IUserContext.BypassTenantQueryFilter"/>).
    /// </summary>
    protected bool IsTenantQueryFilterSuppressed => IsTenantQueryFilterSuppressedCore;

    /// <summary>Tenant id used by global query filters; mirrors <see cref="IUserContext.TenantId"/>.</summary>
    protected int? TenantIdForGlobalQueryFilter => userContext?.TenantId;

    /// <summary>Runs after tenant defaults are applied and before audit fields are applied.</summary>
    protected virtual void OnBeforeSaveChanges()
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyTenantScopedQueryFilters(this);
    }

    private void ApplyTenantIdForScopedEntities()
    {
        var tenantId = userContext?.TenantId;
        foreach (var entry in ChangeTracker.Entries<ITenantScoped>())
        {
            if (entry.State is not (EntityState.Added or EntityState.Modified))
                continue;

            if (entry.Entity.TenantId != 0)
                continue;

            if (tenantId is null or 0)
                throw new InvalidOperationException(
                    "Cannot save tenant-scoped entity without TenantId or authenticated tenant context.");

            entry.Entity.TenantId = tenantId.Value;
        }

        foreach (var entry in ChangeTracker.Entries<ITenantScopedWithSharedRows>())
        {
            if (entry.State is not (EntityState.Added or EntityState.Modified))
                continue;

            // null = system catalog row; > 0 = tenant row already set.
            if (entry.Entity.TenantId is null or > 0)
                continue;

            if (tenantId is null or 0)
                throw new InvalidOperationException(
                    "Cannot save tenant-scoped entity without TenantId or authenticated tenant context.");

            entry.Entity.TenantId = tenantId.Value;
        }
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        ApplyTenantIdForScopedEntities();
        OnBeforeSaveChanges();
        this.ApplyAutomaticAuditFields(userContext);
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        ApplyTenantIdForScopedEntities();
        OnBeforeSaveChanges();
        this.ApplyAutomaticAuditFields(userContext);
        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
}
