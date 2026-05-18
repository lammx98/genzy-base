namespace Genzy.Base.Models;

/// <summary>Audit fields + tenant scope with optional system-wide catalog rows (<see cref="TenantId"/> null).</summary>
public abstract class BaseTenantAudit2WithSharedRows : BaseAudit2, ITenantScopedWithSharedRows
{
    public int? TenantId { get; set; }
}
