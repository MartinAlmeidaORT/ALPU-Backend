using Domain.Models;

namespace Domain.Interfaces.Public.Repositories;

public interface IUserRepository
{
    public IQueryable<User> GetAllUsers();

    public Task<User?> GetUserByIdAsync(int id);

    public User DeleteUser(User user);

    public Task<User?> GetUserByEmailAsync(string email);

    public Task<User?> GetUserByGoogleIdOrEmailAsync(string googleId, string email);
}
