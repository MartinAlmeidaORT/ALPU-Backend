using DataAccess.EntityFramework;
using Domain.Classes.Abstracts;
using Domain.Interfaces.Public.Repositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class UserRepository(DatabaseContext context) : RepositoryBase<User>(context), IUserRepository
{
    public IQueryable<User> GetAllUsers() => GetAll();

    public async Task<User?> GetUserByIdAsync(int id) => await Get(id);

    public User DeleteUser(User entity) => Delete(entity);

    public Task<User?> GetUserByEmailAsync(string email) => context.Users.FirstOrDefaultAsync(u => u.Email == email);

    public Task<User?> GetUserByGoogleIdOrEmailAsync(string googleId, string email) => context.Users.FirstOrDefaultAsync(user => user.GoogleId == googleId || user.Email == email);
}
