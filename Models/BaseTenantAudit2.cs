namespace Genzy.Base.Models;

/// <summary>Audit fields (including actor ids) + tenant scope.</summary>
public abstract class BaseTenantAudit2 : BaseAudit2, ITenantScoped
{
    public string TenantId { get; set; } = string.Empty;
}
