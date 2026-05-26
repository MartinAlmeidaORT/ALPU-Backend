using Domain.Common;

namespace Domain.Models;

public class Region : Entity
{
    internal Region() { }

    public int RegionId { get; set; }

    public decimal Multiplier { get; set; }

    public virtual ICollection<Country> Countries { get; set; } = [];
}
