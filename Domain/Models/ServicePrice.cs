using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Classes;
using Microsoft.EntityFrameworkCore;

namespace Domain.Models;

[PrimaryKey("ServiceId", "DurationId")]
[Table("service_price")]
public partial class ServicePrice : Entity
{
    [Key]
    [Column("service_id")]
    public int ServiceId { get; set; }

    [Key]
    [Column("duration_id")]
    public int DurationId { get; set; }

    [Column("price")]
    public decimal Price { get; set; }

    [Column("variant_price")]
    public decimal? VariantPrice { get; set; }

    [ForeignKey("ServiceId")]
    [InverseProperty("ServicePrices")]
    public virtual ServiceDuration Service { get; set; } = null!;

    [ForeignKey("DurationId")]
    [InverseProperty("ServicePrices")]
    public virtual Duration Duration { get; set; } = null!;
}
