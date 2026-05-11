using Genzy.Base.Security;
using Microsoft.EntityFrameworkCore;

namespace Genzy.Base.Data;

/// <summary>
/// <see cref="BaseDbContext{T}"/> that automatically applies <see cref="DbContextAuditExtensions.ApplyAutomaticAuditFields"/> on save.
/// For contexts that should not auto-audit, use <see cref="BaseDbContext{T}"/> and optionally call the extension manually.
/// </summary>
public abstract class AuditableDbContext<T>(
    DbContextOptions<T> options,
    IUserContext? userContext = null) : BaseDbContext<T>(options)
    where T : DbContext
{
    /// <summary>User context used for <see cref="BaseAudit2"/> actor fields; may be null (e.g. design-time, background jobs).</summary>
    protected IUserContext? AuditUserContext => userContext;

    /// <summary>Runs before audit fields are applied (e.g. tenant id stamping).</summary>
    protected virtual void OnBeforeSaveChanges()
    {
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        OnBeforeSaveChanges();
        this.ApplyAutomaticAuditFields(userContext);
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        OnBeforeSaveChanges();
        this.ApplyAutomaticAuditFields(userContext);
        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
}
