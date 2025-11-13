using System.ComponentModel.DataAnnotations.Schema;

namespace Genzy.Base.Models;

public class BaseModel
{
    [Column("id")]
    public virtual ulong Id { get; set; }
}