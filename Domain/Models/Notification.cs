using Domain.Common;
using Domain.Common.Errors;
using FluentResults;

namespace Domain.Models;

public class Notification : Entity
{
    internal Notification() { }

    public static Result<Notification> CreateNotification(string title, string description)
    {
        Notification notification = new()
        {
            Title = title,
            Description = description,
            Date = DateTime.UtcNow
        };
        return notification.Validate();
    }

    public Result<Notification> Validate()
    {
        if (string.IsNullOrEmpty(Title))
        {
            return Result.Fail(NotificationErrors.TitleIsRequired());
        }

        return Result.Ok(this);
    }

    public int NotificationId { get; set; }

    public int UserId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime Date { get; set; }

    public bool? IsRead { get; set; }

    public virtual User User { get; set; } = null!;
}

public static class NotificationErrors
{
    public class NotificationNotFoundError(string msg) : NotFoundError(msg);

    public class TitleIsRequiredError(string msg) : ValidationError(msg);

    public static NotificationNotFoundError NotificationNotFound(int id) => new($"Notificación con {id} no encontrada.");

    public static TitleIsRequiredError TitleIsRequired() => new("Las notificaciones necesitan tener un titulo.");
}
