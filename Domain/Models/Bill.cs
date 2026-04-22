using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Classes;
using Domain.Enums;

namespace Domain.Models;

[Table("bill")]
public partial class Bill : Entity
{
    [Key]
    [Column("bill_id")]
    public int BillId { get; set; }

    [Column("type", TypeName = "bill_type_enum")]
    [EnumDataType(typeof(BillType))]
    public BillType State { get; set; }

    [Column("contract_id")]
    public int? ContractId { get; set; }

    [Column("title")]
    [StringLength(200)]
    public string Title { get; set; } = null!;

    [Column("description")]
    [StringLength(500)]
    public string Description { get; set; } = null!;

    [Column("date")]
    public DateOnly Date { get; set; }

    [Column("amount")]
    public decimal Amount { get; set; }

    [Column("proof_file")]
    [StringLength(200)]
    public string ProofFile { get; set; } = null!;

    [ForeignKey("ContractId")]
    [InverseProperty("Bills")]
    public virtual Contract? Contract { get; set; }
}
