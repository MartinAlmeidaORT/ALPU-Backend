namespace Domain.Models.Services;

public class IvrService : BaseService
{
    public decimal InitialMessagePrice => (decimal)BasePrice;

    public decimal AdditionalMessagePrice => (decimal)ExtraPrice;

    public decimal UpdateMessagePrice { get; set; }

    public virtual ICollection<RangeIvr> RangeIvr { get; set; } = [];
}
