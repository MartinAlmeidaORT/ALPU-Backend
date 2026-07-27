using Domain.Common.Inputs;
using Domain.Common.Payloads;
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

    Task<Result<Notification[]>> DeleteAllNotificationsAsync(int userId);

    Task<Result<ProfilePictureUploadPayload>> RequestProfilePictureUploadUrlAsync(int userId, string fileName);

    Task<Result<User>> ConfirmProfilePictureUploadAsync(int userId, string key);

    Task<Result<DemoUploadPayload>> RequestDemoUploadUrlAsync(int broadcasterId, string fileName);

    Task<Result<Demo>> ConfirmDemoUploadAsync(int broadcasterId, string key, int languageId, string title);

    Task<Result<Demo>> DeleteDemoAsync(int broadcasterId, string key);

    Task<Result<Broadcaster>> UpdateBroadcasterProfileAsync(int broadcasterId, UpdateBroadcasterProfileInput input);
}
