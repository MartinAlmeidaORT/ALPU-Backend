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

    public async Task<User> DeleteUserAsync(int id)
    {
        User? user = await unitOfWork.Users.GetUserByIdAsync(id) ?? throw new KeyNotFoundException($"User with id {id} not found.");
        unitOfWork.Users.DeleteUser(user);
        await unitOfWork.SaveChangesAsync();
        return user;
    }
}
