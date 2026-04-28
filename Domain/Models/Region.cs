using Domain.Common;

namespace Domain.Models;

public partial class Region : Entity
{
    public int RegionId { get; set; }

    public decimal Multiplier { get; set; }

    public virtual ICollection<Country> Countries { get; set; } = [];
}
