using Domain.Common;

namespace Domain.Models;

public class Notification : Entity
{
    internal Notification() { }

    public int NotificationId { get; set; }

    public int UserId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime Date { get; set; }

    public bool? IsRead { get; set; }

    public virtual User User { get; set; } = null!;
}
