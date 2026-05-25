using Domain.Models;

namespace Domain.Interfaces.Public.Repositories;

public interface IUserRepository
{
    public IQueryable<User> GetAllUsers();

    public IQueryable<Client> GetAllClients();

    public IQueryable<Broadcaster> GetAllBroadcasters();

    public Task<User?> GetUserByIdAsync(int id);

    public User DeleteUser(User user);

    public Task<User?> GetUserByEmailAsync(string email);

    public Task<User?> GetUserByRutAsync(string rut);


    public Task<User?> GetUserByGoogleIdAsync(string googleId);
}
