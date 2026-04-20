using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models;

[Table("service_special")]
public partial class ServiceSpecial : Service
{
    [Column("price")]
    public decimal Price { get; set; }
}
