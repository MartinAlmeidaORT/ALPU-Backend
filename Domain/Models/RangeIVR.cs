using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Domain.Models;

[PrimaryKey("ServiceId", "MinWord")]
[Table("range_ivr")]
public partial class RangeIVR : Entity
{
    [Key]
    [Column("service_id")]
    public int ServiceId { get; set; }

    [Key]
    [Column("min_word")]
    public int MinWord { get; set; }

    [Column("max_word")]
    public int? MaxWord { get; set; }

    [Column("price_per_word")]
    public decimal PricePerWord { get; set; }

    [ForeignKey("ServiceId")]
    [InverseProperty("RangeIVR")]
    public virtual ServiceIVR Service { get; set; } = null!;
}
