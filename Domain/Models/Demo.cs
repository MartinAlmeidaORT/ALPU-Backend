using Domain.Common;

namespace Domain.Models;

public partial class Demo : Entity
{
    public int BroadcasterId { get; set; }

    public string FileName { get; set; } = null!;

    public virtual Broadcaster Broadcaster { get; set; } = null!;
}
