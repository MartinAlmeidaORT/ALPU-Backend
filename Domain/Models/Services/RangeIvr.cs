using Domain.Common;

namespace Domain.Models.Services;

public class RangeIvr : Entity
{
    public int ServiceId { get; set; }

    public int MinWord { get; set; }

    public int? MaxWord { get; set; }

    public decimal PricePerWord { get; set; }

    public IvrService Service { get; set; } = null!;
}
