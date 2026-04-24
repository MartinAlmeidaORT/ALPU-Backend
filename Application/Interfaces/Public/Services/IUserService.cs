using Application.DTOs.Users;
using Domain.Models;

namespace Application.Interfaces.Public.Services;

public interface IUserService
{
    IQueryable<User> GetAllUsers();

    Task<User?> GetUserByIdAsync(int id);

    Task<User> UpdateUserAsync(int id, UpdateUserDTO dto);

    Task<User> DeleteUserAsync(int id);
}
