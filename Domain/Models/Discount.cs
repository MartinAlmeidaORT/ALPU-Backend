using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models;

[Table("discount")]
public partial class Discount
{
    [Key]
    [Column("discount_id")]
    public int DiscountId { get; set; }

    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Column("amount")]
    public decimal Amount { get; set; }

    [ForeignKey("DiscountId")]
    [InverseProperty("Discounts")]
    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();
}
