using DataAccess.EntityFramework;
using Domain.Interfaces.Public.Repositories;
using Domain.Models;

namespace DataAccess.Repositories;

public class ClientRepository(DatabaseContext context) : IClientRepository
{
    public Task<User> Create(User entity)
    {
        throw new NotImplementedException();
    }

    public async Task<Agency> CreateAgency(Agency entity)
    {
        await context.Agencies.AddAsync(entity);
        return entity;
    }

    public Task<User> Delete(User entity)
    {
        throw new NotImplementedException();
    }

    public async Task<Agency?> GetAgencyByIdAsync(int id) => await context.Agencies.FindAsync(id);

    public IQueryable<User> GetAll()
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<User> Update(User entity)
    {
        throw new NotImplementedException();
    }
}
