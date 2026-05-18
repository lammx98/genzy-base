namespace Genzy.Base.Models;

/// <summary>
/// Marks a <see cref="ITenantScopedWithSharedRows"/> entity whose global query filter is applied by the host <see cref="Microsoft.EntityFrameworkCore.DbContext"/>
/// (e.g. to exclude per-tenant disabled system catalog rows).
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class SharedCatalogEntityAttribute : Attribute;
