using Domain.Common;

namespace Domain.Models;

public partial class RangeIVR : Entity
{
    public int ServiceId { get; set; }

    public int MinWord { get; set; }

    public int? MaxWord { get; set; }

    public decimal PricePerWord { get; set; }

    public virtual ServiceIVR Service { get; set; } = null!;
}
