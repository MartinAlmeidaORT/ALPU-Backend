using Domain.Models;
using Domain.Interfaces.Public.Repositories;
using Domain.Common.Inputs;
using FluentResults;
using Domain.Interfaces.Public.Services;
using Microsoft.EntityFrameworkCore;
using HotChocolate.Subscriptions;
using Domain.Interfaces.Private;
using DataAccess.ExternalServices;
using Domain.Common.Payloads;

namespace Application.Services;

public class UserService(IUnitOfWork unitOfWork, ITopicEventSender sender, IEmailService emailService, AmazonS3Service amazonS3Service) : IUserService
{
    private readonly ITopicEventSender _sender = sender;

    private readonly IEmailService _emailService = emailService;

    private readonly AmazonS3Service _amazonS3Service = amazonS3Service;

    public IQueryable<User> GetAllUsers() => unitOfWork.Users.GetAllUsers();

    public IQueryable<Client> GetAllClients() => unitOfWork.Users.GetAllClients();

    public IQueryable<Broadcaster> GetAllBroadcasters() => unitOfWork.Users.GetAllBroadcasters();

    public async Task<User?> GetUserByIdAsync(int id) => await unitOfWork.Users.GetUserByIdAsync(id);

    public async Task<User> UpdateUserAsync(int id, UpdateUserInput dto)
    {
        User? user = await unitOfWork.Users.GetUserByIdAsync(id) ?? throw new KeyNotFoundException($"User with id {id} not found.");

        Country? country = null;
        if (dto.Address?.CountryCode != null)
        {
            country = await unitOfWork.Countries.GetByCodeAsync(dto.Address.CountryCode) ?? throw new KeyNotFoundException($"Country with code {dto.Address.CountryCode} not found.");
        }

        Department? department = null;
        if (dto.Address?.DepartmentId != null)
        {
            int departmentId = (int)dto.Address.DepartmentId;
            department = await unitOfWork.Departments.GetByIdAsync(departmentId) ?? throw new KeyNotFoundException($"Department with id {dto.Address.DepartmentId} not found.");
        }

        user.Update(dto, country, department);
        await unitOfWork.SaveChangesAsync();

        return user;
    }

    public async Task<Result> ApproveUser(UpdateUserStateInput input)
    {
        User? user = unitOfWork.Users.GetAllUsers()
            .Where(u => u.UserId == input.UserId && (u is Client || u is Broadcaster))
            .SingleOrDefault();

        if (user == null)
        {
            return Result.Fail(UserErrors.UserNotFound(input.UserId));
        }

        user.UserState = input.NewState;
        await unitOfWork.SaveChangesAsync();
        await _emailService.SendAccountApprovedAsync(user.Email, user.FullName);
        return Result.Ok();
    }

    public async Task<User> DeleteUserAsync(int id)
    {
        User? user = await unitOfWork.Users.GetUserByIdAsync(id) ?? throw new KeyNotFoundException($"User with id {id} not found.");
        unitOfWork.Users.DeleteUser(user);
        await unitOfWork.SaveChangesAsync();
        return user;
    }

    public IQueryable<Notification> GetUserNotifications(int userId)
    {
        var user = unitOfWork.Users.GetAllUsers()
            .Include(u => u.Notifications)
            .SingleOrDefault(u => u.UserId == userId);

        if (user?.Notifications == null)
        {
            return Enumerable.Empty<Notification>().AsQueryable();
        }

        return user.Notifications.AsQueryable();
    }

    public async Task AddNotificationAsync(User user, string title, string description)
    {
        var notification = user.AddNotification(title, description);

        if (notification.IsFailed)
        {
            Console.WriteLine($"Fail to create notification.\nTitle: {title}\nDescription: {description}");
            Console.WriteLine(notification.Errors);
            return;
        }
        await _sender.SendAsync($"{user.UserId}", notification.Value);
    }

    public async Task<Result<Notification>> DeleteNotificationAsync(int userId, int notificationId)
    {
        User? user = await unitOfWork.Users.GetAllUsers().Include(u => u.Notifications).SingleOrDefaultAsync(u => u.UserId == userId);
        if (user == null)
        {
            return Result.Fail(UserErrors.UserNotFound(userId));
        }

        Result<Notification> result = user.RemoveNotification(notificationId);
        if (result.IsSuccess)
        {
            await unitOfWork.SaveChangesAsync();
        }
        return result;
    }

    public async Task<Result<Notification[]>> DeleteAllNotificationsAsync(int userId)
    {
        User? user = await unitOfWork.Users.GetAllUsers().Include(u => u.Notifications).SingleOrDefaultAsync(u => u.UserId == userId);
        if (user == null)
        {
            return Result.Fail(UserErrors.UserNotFound(userId));
        }

        Result<Notification[]> result = user.ClearNotifications();
        if (result.IsSuccess)
        {
            await unitOfWork.SaveChangesAsync();
        }
        return result;
    }

    public async Task<Result<ProfilePictureUploadPayload>> RequestProfilePictureUploadUrlAsync(int userId, string fileName)
    {
        User? user = await unitOfWork.Users.GetUserByIdAsync(userId);
        if (user == null)
        {
            return Result.Fail(UserErrors.UserNotFound(userId));
        }

        Result<(string Key, string UploadUrl)> upload = _amazonS3Service.SaveProfilePictureAsync(fileName, userId);
        if (upload.IsFailed)
        {
            return Result.Fail(upload.Errors);
        }

        return new ProfilePictureUploadPayload
        {
            Key = upload.Value.Key,
            UploadUrl = upload.Value.UploadUrl
        };
    }

    public async Task<Result<User>> ConfirmProfilePictureUploadAsync(int userId, string key)
    {
        User? user = await unitOfWork.Users.GetUserByIdAsync(userId);
        if (user == null)
        {
            return Result.Fail(UserErrors.UserNotFound(userId));
        }

        string? previousPhoto = user.Photo;
        user.Photo = key;
        await unitOfWork.SaveChangesAsync();

        // SaveProfilePictureAsync uses a fixed per-user key, so this only differs (and needs cleanup)
        // if the file extension changed between uploads (e.g. a .png replaced by a .jpg).
        if (!string.IsNullOrEmpty(previousPhoto) && previousPhoto != key)
        {
            await _amazonS3Service.DeleteProfilePictureAsync(previousPhoto);
        }

        return user;
    }

    public async Task<Result<DemoUploadPayload>> RequestDemoUploadUrlAsync(int broadcasterId, string fileName)
    {
        Broadcaster? broadcaster = await unitOfWork.Broadcasters.GetBroadcasterByIdAsync(broadcasterId);
        if (broadcaster == null)
        {
            return Result.Fail(UserErrors.UserNotFound(broadcasterId));
        }

        Result<(string Key, string UploadUrl)> upload = _amazonS3Service.SaveDemoAsync(fileName, broadcasterId);
        if (upload.IsFailed)
        {
            return Result.Fail(upload.Errors);
        }

        return new DemoUploadPayload
        {
            Key = upload.Value.Key,
            UploadUrl = upload.Value.UploadUrl
        };
    }

    public async Task<Result<Demo>> ConfirmDemoUploadAsync(int broadcasterId, string key)
    {
        Broadcaster? broadcaster = await unitOfWork.Broadcasters.GetBroadcasterByIdAsync(broadcasterId);
        if (broadcaster == null)
        {
            return Result.Fail(UserErrors.UserNotFound(broadcasterId));
        }

        Result<Demo> result = broadcaster.AddDemo(key);
        if (result.IsFailed)
        {
            return result;
        }

        await unitOfWork.SaveChangesAsync();
        return result;
    }
}
