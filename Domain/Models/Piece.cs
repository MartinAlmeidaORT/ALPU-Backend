using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models;

[Table("piece")]
public partial class Piece
{
    [Key]
    [Column("piece_id")]
    public int PieceId { get; set; }

    [Column("contract_id")]
    public int ContractId { get; set; }

    [Column("name")]
    [StringLength(200)]
    public string Name { get; set; } = null!;

    [Column("service_id")]
    public int ServiceId { get; set; }

    [Column("variants")]
    public int Variants { get; set; }

    [ForeignKey("ContractId")]
    [InverseProperty("Pieces")]
    public virtual Contract Contract { get; set; } = null!;

    [ForeignKey("ServiceId")]
    [InverseProperty("Pieces")]
    public virtual Service Service { get; set; } = null!;

    [ForeignKey("PieceId")]
    [InverseProperty("Pieces")]
    public virtual ICollection<ExtraCharge> ExtraCharges { get; set; } = [];
}
