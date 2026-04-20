using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models;

[Table("service_ivr")]
public partial class ServiceIVR : Service
{
    [Column("initial_message_price")]
    public decimal InitialMessagePrice { get; set; }

    [Column("additional_message_price")]
    public decimal AdditionalMessagePrice { get; set; }

    [Column("update_message_price")]
    public decimal UpdateMessagePrice { get; set; }

    [InverseProperty("Service")]
    public virtual ICollection<RangeIVR> RangeIVR { get; set; } = [];
}
