namespace Domain.Models;

public partial class ServiceNarrative : Service
{
    public decimal BasePrice { get; set; }

    public decimal ExtraPrice { get; set; }

    public decimal RolPrice { get; set; }
}
