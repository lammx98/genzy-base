namespace Genzy.Base.Models;

/// <summary>
/// Shared-catalog row (tenant-owned or system-wide with null <see cref="ITenantScopedWithSharedRows.TenantId"/>).
/// </summary>
public interface ISharedCatalogRow : ITenantScopedWithSharedRows, IActiveCatalogRow
{
    ulong Id { get; set; }
}
