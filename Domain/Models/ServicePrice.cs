using Domain.Common;

namespace Domain.Models;

public partial class ServicePrice : Entity
{
    public int ServiceId { get; set; }

    public int DurationId { get; set; }

    public decimal Price { get; set; }

    public decimal? VariantPrice { get; set; }

    public virtual ServiceDuration Service { get; set; } = null!;

    public virtual Duration Duration { get; set; } = null!;
}
