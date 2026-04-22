using Application.DTOs.Users;

namespace Application.Interfaces.Public.Services;

public interface IUserService
{
    IQueryable<IResultUserDTO> GetAllUsers();

    Task<IResultUserDTO?> GetUserByIdAsync(int id);

    Task<IResultUserDTO> UpdateUserAsync(int id, UpdateUserDTO dto);

    Task<IResultUserDTO> DeleteUserAsync(int id);
}
