using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models;

[Table("service")]
public partial class Service
{
    [Key]
    [Column("service_id")]
    public int ServiceId { get; set; }

    [Column("name")]
    [StringLength(200)]
    public string Name { get; set; } = null!;

    [InverseProperty("Service")]
    public virtual ICollection<Piece> Pieces { get; set; } = [];

    [InverseProperty("Service")]
    public virtual ICollection<VolumeDiscount> VolumeDiscounts { get; set; } = [];
}
