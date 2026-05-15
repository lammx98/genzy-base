namespace Genzy.Base.Models;

/// <summary>
/// When applied to an <see cref="ITenantScoped"/> entity type, no global tenant query filter is registered for that type.
/// Use sparingly for truly global rows (e.g. reference data shared across tenants).
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class SkipTenantQueryFilterAttribute : Attribute;
