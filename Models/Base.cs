using System.ComponentModel.DataAnnotations.Schema;

namespace GCL.Base.Models;

public class BaseModel
{
    [Column("id")]
    public virtual ulong Id { get; set; }
}