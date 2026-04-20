using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models;

[Table("contract")]
public partial class Contract
{
    [Key]
    [Column("contract_id")]
    public int ContractId { get; set; }

    [Column("client_id")]
    public int ClientId { get; set; }

    [Column("broadcaster_id")]
    public int BroadcasterId { get; set; }

    [Column("date")]
    public DateOnly Date { get; set; }

    [Column("due_date")]
    public DateOnly DueDate { get; set; }

    [Column("country_code")]
    [StringLength(3)]
    public string CountryCode { get; set; } = null!;

    [Column("price")]
    public decimal Price { get; set; }

    [Column("discount_id")]
    public int? DiscountId { get; set; }

    [Column("term_years")]
    public int TermYears { get; set; }

    [InverseProperty("Contract")]
    public virtual ICollection<Bill> Bills { get; set; } = [];

    [ForeignKey("BroadcasterId")]
    [InverseProperty("Contracts")]
    public virtual Broadcaster Broadcaster { get; set; } = null!;

    [ForeignKey("ClientId")]
    [InverseProperty("Contracts")]
    public virtual Client Client { get; set; } = null!;

    [ForeignKey("CountryCode")]
    [InverseProperty("Contracts")]
    public virtual Country CountryCodeNavigation { get; set; } = null!;

    [InverseProperty("Contract")]
    public virtual ICollection<Piece> Pieces { get; set; } = [];

    [ForeignKey("ContractId")]
    [InverseProperty("Contracts")]
    public virtual ICollection<Discount> Discounts { get; set; } = [];
}
