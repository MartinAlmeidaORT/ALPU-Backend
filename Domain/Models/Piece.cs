using Domain.Common;

namespace Domain.Models;

public partial class Piece : Entity
{
    public int PieceId { get; set; }

    public int? ContractId { get; set; }

    public string Name { get; set; } = null!;

    public int ServiceId { get; set; }

    public int Variants { get; set; }

    public virtual Contract Contract { get; set; } = null!;

    public virtual Service Service { get; set; } = null!;

    public virtual ICollection<ExtraCharge> ExtraCharges { get; set; } = [];
}
