using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models;

[Table("service_duration")]
public partial class ServiceDuration : Service
{
    [InverseProperty("Service")]
    public virtual ICollection<ServicePrice> ServicePrices { get; set; } = [];
}
