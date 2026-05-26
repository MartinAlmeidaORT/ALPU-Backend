using Domain.Common;

namespace Domain.Models;

public class Demo : Entity
{
    internal Demo() { }

    public int BroadcasterId { get; set; }

    public string FileName { get; set; } = null!;

    public virtual Broadcaster Broadcaster { get; set; } = null!;
}
