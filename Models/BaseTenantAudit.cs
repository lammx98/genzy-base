namespace Genzy.Base.Models;

/// <summary>Audit fields + tenant scope (same <see cref="ITenantScoped"/> contract as <see cref="BaseTenant"/>).</summary>
public abstract class BaseTenantAudit : BaseAudit, ITenantScoped
{
    public int TenantId { get; set; }
}
