using Domain.Common;

namespace Domain.Models;

public partial class ExtraCharge : Entity
{
    public int ExtraChargeId { get; set; }

    public string Name { get; set; } = null!;

    public decimal Amount { get; set; }

    public virtual ICollection<Piece> Pieces { get; set; } = [];
}
