using Domain.Common.Abstracts;
using Domain.Enums;

namespace Domain.Models;

public class Membership : Entity
{
    internal Membership() { }

    public int MembershipId { get; set; }

    public MembershipState State { get; set; }

    public int BroadcasterId { get; set; }

    public DateOnly? PayDate { get; set; }

    public DateOnly? DueDate { get; set; }

    public decimal? Amount { get; set; }

    public virtual Broadcaster Broadcaster { get; set; } = null!;
}
