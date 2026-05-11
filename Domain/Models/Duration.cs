using Domain.Common;

namespace Domain.Models;

public partial class Duration : Entity
{
    public int DurationId { get; set; }

    public string Name { get; set; } = null!;

    public int Time { get; set; }

    public virtual ICollection<ServicePrice> ServicePrices { get; set; } = [];
}
