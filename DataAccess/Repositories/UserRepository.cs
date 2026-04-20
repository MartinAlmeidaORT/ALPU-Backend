using DataAccess.EntityFramework;
using Domain.Interfaces.Public.Repositories;
using Domain.Models;

namespace DataAccess.Repositories;

public class UserRepository(DatabaseContext context) : IUserRepository
{
    public IQueryable<User> GetAll() => context.Users;

    public async Task<User?> GetByIdAsync(int id) => await context.Users.FindAsync(id);

    public async Task<User> Create(User entity)
    {
        await context.Users.AddAsync(entity);
        return entity;
    }

    public async Task<User> Update(User entity)
    {
        context.Users.Update(entity);
        return entity;
    }

    public async Task<User> Delete(User entity)
    {
        context.Users.Remove(entity);
        return entity;
    }
}
