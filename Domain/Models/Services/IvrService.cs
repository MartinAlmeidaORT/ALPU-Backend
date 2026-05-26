namespace Domain.Models.Services;

public class IvrService : BaseService
{
    internal IvrService() { }

    public decimal InitialMessagePrice => BasePrice ?? throw new NullReferenceException();

    public decimal AdditionalMessagePrice => ExtraPrice ?? throw new NullReferenceException();

    public decimal UpdateMessagePrice { get; set; }

    public virtual ICollection<RangeIvr> RangeIvr { get; set; } = [];
}
