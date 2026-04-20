using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Models;

[PrimaryKey("BroadcasterId", "FileName")]
[Table("demo")]
public partial class Demo
{
    [Key]
    [Column("broadcaster_id")]
    public int BroadcasterId { get; set; }

    [Key]
    [Column("file_name")]
    [StringLength(200)]
    public string FileName { get; set; } = null!;

    [ForeignKey("BroadcasterId")]
    [InverseProperty("Demos")]
    public virtual Broadcaster Broadcaster { get; set; } = null!;
}
