using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Classes;
using Microsoft.EntityFrameworkCore;

namespace Domain.Models;

[PrimaryKey("ServiceId", "MinQuantity")]
[Table("volume_discount")]
public partial class VolumeDiscount : Entity
{
    [Key]
    [Column("service_id")]
    public int ServiceId { get; set; }

    [Key]
    [Column("min_quantity")]
    public int MinQuantity { get; set; }

    [Column("discount")]
    public decimal Discount { get; set; }

    [ForeignKey("ServiceId")]
    [InverseProperty("VolumeDiscounts")]
    public virtual Service Service { get; set; } = null!;
}
