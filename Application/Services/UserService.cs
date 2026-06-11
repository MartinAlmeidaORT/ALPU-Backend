using Domain.Models;
using Domain.Interfaces.Public.Repositories;
using Domain.Common.Inputs;
using FluentResults;
using Domain.Interfaces.Public.Services;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class UserService(IUnitOfWork unitOfWork) : IUserService
{
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
}
