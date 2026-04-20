using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models;

[Table("extra_charge")]
public partial class ExtraCharge
{
    [Key]
    [Column("extra_charge_id")]
    public int ExtraChargeId { get; set; }

    [Column("name")]
    [StringLength(200)]
    public string Name { get; set; } = null!;

    [Column("amount")]
    public decimal Amount { get; set; }

    [ForeignKey("ExtraChargeId")]
    [InverseProperty("ExtraCharges")]
    public virtual ICollection<Piece> Pieces { get; set; } = [];
}
