namespace Domain.Models;

public partial class ServiceIVR : Service
{
    public decimal InitialMessagePrice { get; set; }

    public decimal AdditionalMessagePrice { get; set; }

    public decimal UpdateMessagePrice { get; set; }

    public virtual ICollection<RangeIVR> RangeIVR { get; set; } = [];
}
