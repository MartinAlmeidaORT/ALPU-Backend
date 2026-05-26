using Domain.Common;

namespace Domain.Models;

public partial class BroadcasterCategory : Entity
{
    internal BroadcasterCategory() { }

    public int BroadcasterCategoryId { get; set; }

    public string Name { get; set; } = null!;

    public int LifetimeJobCount { get; set; }

    public virtual ICollection<Broadcaster> Broadcasters { get; set; } = [];
}
