namespace Genzy.Base.Models;

/// <summary>Row belongs to a single tenant (shared database, tenant column).</summary>
public interface ITenantScoped
{
    string TenantId { get; set; }
}
