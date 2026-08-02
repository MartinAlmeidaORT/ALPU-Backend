using Domain.Common.Abstracts;

namespace Domain.Models.Services;

public class RangeIvr : Entity
{
    internal RangeIvr() { }

    public int ServiceId { get; set; }

    public int MinWord { get; set; }

    public int? MaxWord { get; set; }

    public decimal PricePerWord { get; set; }

    public IvrService Service { get; set; } = null!;
}
