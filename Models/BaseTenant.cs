namespace Genzy.Base.Models;

/// <summary>Primary key (<see cref="BaseModel.Id"/>) + tenant scope for shared-schema tables.</summary>
public abstract class BaseTenant : BaseModel, ITenantScoped
{
    public string TenantId { get; set; } = string.Empty;
}
