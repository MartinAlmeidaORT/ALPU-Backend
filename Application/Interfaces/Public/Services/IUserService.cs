using Domain.Common.Inputs;
using Domain.Models;

namespace Application.Interfaces.Public.Services;

public interface IUserService
{
    IQueryable<User> GetAllUsers();

    Task<User?> GetUserByIdAsync(int id);

    Task<User> UpdateUserAsync(int id, UpdateUserInput dto);

    Task<User> DeleteUserAsync(int id);
}
