using Domain.Common;

namespace Domain.Models;

public partial class Discount : Entity
{
    public int DiscountId { get; set; }

    public string Name { get; set; } = null!;

    public decimal Amount { get; set; }

    public virtual ICollection<Contract> Contracts { get; set; } = [];
}
