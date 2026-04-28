namespace Domain.Models;

public partial class ServiceDuration : Service
{
    public virtual ICollection<ServicePrice> ServicePrices { get; set; } = [];
}
