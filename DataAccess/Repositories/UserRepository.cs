using DataAccess.EntityFramework;
using Domain.Common.Abstracts;
using Domain.Interfaces.Public.Repositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class UserRepository(DatabaseContext context) : RepositoryBase<User>(context), IUserRepository
{
    public IQueryable<User> GetAllUsers() => GetAll();

    public IQueryable<Client> GetAllClients() => context.Clients;
    public IQueryable<Broadcaster> GetAllBroadcasters() => context.Broadcasters;
    public async Task<User?> GetUserByIdAsync(int id) => await Get(id);

    public User DeleteUser(User entity) => Delete(entity);

    public Task<User?> GetUserByEmailAsync(string email) => context.Users.FirstOrDefaultAsync(u => u.Email == email);

    public Task<User?> GetUserByRutAsync(string rut) => context.Users.FirstOrDefaultAsync(u => u.RUT == rut);


    public Task<User?> GetUserByGoogleIdAsync(string googleId) => context.Users.FirstOrDefaultAsync(user => user.GoogleId == googleId);
}
