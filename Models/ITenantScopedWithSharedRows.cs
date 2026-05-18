namespace Genzy.Base.Models;

/// <summary>
/// Tenant-owned rows plus optional system-wide catalog rows (<see cref="TenantId"/> null).
/// Global query filters include both the current tenant and shared rows; CUD paths should target tenant rows only.
/// </summary>
public interface ITenantScopedWithSharedRows
{
    int? TenantId { get; set; }
}
