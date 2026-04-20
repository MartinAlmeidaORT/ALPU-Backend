using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models;

[Table("service_narrative")]
public partial class ServiceNarrative : Service
{
    [Column("base_price")]
    public decimal BasePrice { get; set; }

    [Column("extra_price")]
    public decimal ExtraPrice { get; set; }

    [Column("rol_price")]
    public decimal RolPrice { get; set; }
}
