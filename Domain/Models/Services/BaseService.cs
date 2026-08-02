using Domain.Common.Abstracts;
using Domain.Enums;

namespace Domain.Models.Services;

public abstract class BaseService : Entity
{
    public int ServiceId { get; set; }

    public string Discriminator { get; set; } = null!;

    public string Name { get; set; } = null!;

    public ServiceType Type { get; set; }

    public decimal? BasePrice { get; set; }

    public decimal? ExtraPrice { get; set; }

    public decimal? FirstExtraPrice { get; set; }
}
