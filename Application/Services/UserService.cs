using Domain.Models;
using Application.Interfaces.Public.Services;
using Domain.Interfaces.Public.Repositories;
using Domain.Common.Inputs;

namespace Application.Services;

public class UserService(IUnitOfWork unitOfWork) : IUserService
{
    public IQueryable<User> GetAllUsers() => unitOfWork.Users.GetAllUsers();

    public async Task<User?> GetUserByIdAsync(int id) => await unitOfWork.Users.GetUserByIdAsync(id);

    public async Task<User> UpdateUserAsync(int id, UpdateUserInput dto)
    {
        User? user = await unitOfWork.Users.GetUserByIdAsync(id) ?? throw new KeyNotFoundException($"User with id {id} not found.");

        Country? country = null;
        if (dto.Address?.CountryCode != null)
        {
            country = await unitOfWork.Countries.GetByCodeAsync(dto.Address.CountryCode) ?? throw new KeyNotFoundException($"Country with code {dto.Address.CountryCode} not found.");
        }

        user.Update(dto, country);
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
