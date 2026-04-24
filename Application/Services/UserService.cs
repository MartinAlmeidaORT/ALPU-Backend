using Domain.Models;
using Application.Interfaces.Public.Services;
using Application.DTOs.Users;
using Application.Mappers;
using Domain.Interfaces.Public.Repositories;

namespace Application.Services;

public class UserService(IUnitOfWork unitOfWork) : IUserService
{
    public IQueryable<User> GetAllUsers() => unitOfWork.Users.GetAllUsers();

    public async Task<User?> GetUserByIdAsync(int id) => await unitOfWork.Users.GetUserByIdAsync(id);

    public async Task<User> UpdateUserAsync(int id, UpdateUserDTO dto)
    {
        User? user = await unitOfWork.Users.GetUserByIdAsync(id) ?? throw new KeyNotFoundException($"User with id {id} not found.");

        Country? country = null;
        if (dto.CountryCode != null)
        {
            country = await unitOfWork.Countries.GetByCodeAsync(dto.CountryCode) ?? throw new KeyNotFoundException($"Country with code {dto.CountryCode} not found.");
        }

        UserMapper.ApplyUpdate(user, dto, country);
        await unitOfWork.SaveChangesAsync();

        return user;
    }

    public async Task<User> DeleteUserAsync(int id)
    {
        User? user = await unitOfWork.Users.GetUserByIdAsync(id) ?? throw new KeyNotFoundException($"User with id {id} not found.");
        unitOfWork.Users.DeleteUser(user);
        await unitOfWork.SaveChangesAsync();
        return user;
    }
}
