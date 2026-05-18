using System.Reflection;
using Genzy.Base.Models;
using Microsoft.EntityFrameworkCore;

namespace Genzy.Base.Data;

public static class TenantScopedQueryFilterExtensions
{
    private static readonly MethodInfo SetTenantFilterMethod =
        typeof(TenantScopedQueryFilterExtensions).GetMethod(nameof(SetTenantFilter), BindingFlags.NonPublic | BindingFlags.Static)
        ?? throw new InvalidOperationException(nameof(SetTenantFilter));

    private static readonly MethodInfo SetSharedRowsTenantFilterMethod =
        typeof(TenantScopedQueryFilterExtensions).GetMethod(nameof(SetSharedRowsTenantFilter), BindingFlags.NonPublic | BindingFlags.Static)
        ?? throw new InvalidOperationException(nameof(SetSharedRowsTenantFilter));

    /// <summary>
    /// Applies a global query filter to every mapped CLR type that implements <see cref="ITenantScoped"/>
    /// or <see cref="ITenantScopedWithSharedRows"/>, except types marked with <see cref="SkipTenantQueryFilterAttribute"/>.
    /// </summary>
    public static void ApplyTenantScopedQueryFilters(this ModelBuilder modelBuilder, ITenantGlobalQueryFilterContext filterContext)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        ArgumentNullException.ThrowIfNull(filterContext);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (entityType.IsOwned())
                continue;

            var clr = entityType.ClrType;
            if (clr is null)
                continue;

            if (Attribute.IsDefined(clr, typeof(SkipTenantQueryFilterAttribute), inherit: false))
                continue;

            if (typeof(ITenantScopedWithSharedRows).IsAssignableFrom(clr))
            {
                if (Attribute.IsDefined(clr, typeof(SharedCatalogEntityAttribute), inherit: false))
                    continue;

                SetSharedRowsTenantFilterMethod.MakeGenericMethod(clr).Invoke(null, [modelBuilder, filterContext]);
                continue;
            }

            if (!typeof(ITenantScoped).IsAssignableFrom(clr))
                continue;

            SetTenantFilterMethod.MakeGenericMethod(clr).Invoke(null, [modelBuilder, filterContext]);
        }
    }

    private static void SetTenantFilter<TEntity>(ModelBuilder modelBuilder, ITenantGlobalQueryFilterContext ctx)
        where TEntity : class, ITenantScoped
    {
        // Fail closed: missing tenant (and no suppression) matches no rows — never cross-tenant reads.
        modelBuilder.Entity<TEntity>().HasQueryFilter(e =>
            ctx.IsTenantQueryFilterSuppressed
            || (ctx.TenantIdForGlobalQueryFilter != null
                && ctx.TenantIdForGlobalQueryFilter > 0
                && e.TenantId == ctx.TenantIdForGlobalQueryFilter));
    }

    private static void SetSharedRowsTenantFilter<TEntity>(ModelBuilder modelBuilder, ITenantGlobalQueryFilterContext ctx)
        where TEntity : class, ITenantScopedWithSharedRows
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(e =>
            ctx.IsTenantQueryFilterSuppressed
            || (ctx.TenantIdForGlobalQueryFilter != null
                && ctx.TenantIdForGlobalQueryFilter > 0
                && (e.TenantId == null || e.TenantId == ctx.TenantIdForGlobalQueryFilter)));
    }
}
