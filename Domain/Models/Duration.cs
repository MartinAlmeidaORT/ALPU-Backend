using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Classes;

namespace Domain.Models;

[Table("duration")]
public partial class Duration : Entity
{
    [Key]
    [Column("duration_id")]
    public int DurationId { get; set; }

    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Column("time")]
    public int Time { get; set; }

    [InverseProperty("Duration")]
    public virtual ICollection<ServicePrice> ServicePrices { get; set; } = [];
}
