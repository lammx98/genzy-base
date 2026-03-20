namespace Genzy.Base.Models;

public class BaseAudit : BaseModel
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class BaseAudit2 : BaseAudit
{
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}