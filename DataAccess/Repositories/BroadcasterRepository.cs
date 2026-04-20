using DataAccess.EntityFramework;
using Domain.Interfaces.Public.Repositories;
using Domain.Models;

namespace DataAccess.Repositories;

public class BroadcasterRepository(DatabaseContext context) : IBroadcasterRepository
{
    public Task<User> Create(User entity)
    {
        throw new NotImplementedException();
    }

    public Task<User> Delete(User entity)
    {
        throw new NotImplementedException();
    }

    public IQueryable<User> GetAll()
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<BroadcasterCategory?> GetCategoryByIdAsync(int id) => await context.BroadcasterCategories.FindAsync(id);

    public Task<User> Update(User entity)
    {
        throw new NotImplementedException();
    }
}
