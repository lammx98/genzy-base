using Genzy.Base.Models;
using Genzy.Base.Security;
using Microsoft.EntityFrameworkCore;

namespace Genzy.Base.Data;

/// <summary>
/// Optional automatic audit: <see cref="BaseAudit"/> / <see cref="BaseAudit2"/> timestamps and actor ids from <see cref="IUserContext"/>.
/// Call from <see cref="DbContext.SaveChanges()"/> / <see cref="DbContext.SaveChangesAsync(System.Threading.CancellationToken)"/> before persisting, or inherit <see cref="AuditableDbContext{T}"/>.
/// </summary>
public static class DbContextAuditExtensions
{
    /// <summary>
    /// Sets <c>CreatedAt</c>/<c>UpdatedAt</c> (and <c>CreatedBy</c>/<c>UpdatedBy</c> for <see cref="BaseAudit2"/>) on tracked <see cref="BaseAudit"/> entities.
    /// </summary>
    public static void ApplyAutomaticAuditFields(this DbContext context, IUserContext? userContext)
    {
        var utcNow = DateTime.UtcNow;
        var actor = userContext?.UserId;

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.Entity is not BaseAudit audit)
                continue;

            switch (entry.State)
            {
                case EntityState.Added:
                    audit.CreatedAt = utcNow;
                    audit.UpdatedAt = utcNow;
                    if (entry.Entity is BaseAudit2 audit2)
                    {
                        audit2.CreatedBy = actor;
                        audit2.UpdatedBy = actor;
                    }
                    break;
                case EntityState.Modified:
                    audit.UpdatedAt = utcNow;
                    if (entry.Entity is BaseAudit2 audit2m)
                        audit2m.UpdatedBy = actor;
                    break;
            }
        }
    }
}
