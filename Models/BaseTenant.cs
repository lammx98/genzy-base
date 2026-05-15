namespace Genzy.Base.Models;

/// <summary>
/// Primary key (<see cref="BaseModel.Id"/>) + tenant scope for shared-schema tables.
/// When used with <see cref="Data.AuditableDbContext{T}"/>, EF Core global query filters restrict reads/updates/deletes
/// to the current <see cref="Security.IUserContext.TenantId"/>, and <see cref="Microsoft.EntityFrameworkCore.DbContext.SaveChanges"/>
/// stamps <see cref="TenantId"/> on new or updated rows when it is unset (0).
/// </summary>
public abstract class BaseTenant : BaseModel, ITenantScoped
{
    public int TenantId { get; set; }
}
