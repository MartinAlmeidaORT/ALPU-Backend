using Domain.Common;

namespace Domain.Models;

public partial class Country : Entity
{
    public string CountryCode { get; set; } = null!;

    public int? RegionId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Contract> Contracts { get; set; } = [];

    public virtual Region? Region { get; set; }
}
