using Domain.Common.Inputs;
using Domain.Models;
using FluentResults;

namespace Domain.Interfaces.Public.Services;

public interface IUserService
{
    IQueryable<User> GetAllUsers();

    IQueryable<Client> GetAllClients();

    IQueryable<Broadcaster> GetAllBroadcasters();

    Task<User?> GetUserByIdAsync(int id);

    Task<User> UpdateUserAsync(int id, UpdateUserInput dto);

    Task<Result> ApproveUser(UpdateUserStateInput input);

    Task<User> DeleteUserAsync(int id);

    IQueryable<Notification> GetUserNotifications(int id);

    Task AddNotificationAsync(User user, string title, string description);

    Task<Result<Notification>> DeleteNotificationAsync(int userId, int notificationId);
}
