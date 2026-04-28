using Domain.Common;

namespace Domain.Models;

public partial class Service : Entity
{
    public int ServiceId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Piece> Pieces { get; set; } = [];

    public virtual ICollection<VolumeDiscount> VolumeDiscounts { get; set; } = [];
}
