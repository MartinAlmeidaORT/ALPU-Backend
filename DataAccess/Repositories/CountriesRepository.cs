using DataAccess.EntityFramework;
using Domain.Interfaces.Public.Repositories;
using Domain.Models;

namespace DataAccess.Repositories;

public class CountryRepository(DatabaseContext context) : ICountryRepository
{
    public IQueryable<Country> GetAll() => context.Countries;

    public async Task<Country?> GetByCodeAsync(string code) => await context.Countries.FindAsync(code);

    public async Task<Country?> GetByIdAsync(int id) => throw new NotImplementedException();

    public async Task<Country> Create(Country entity)
    {
        await context.Countries.AddAsync(entity);
        return entity;
    }

    public async Task<Country> Update(Country entity)
    {
        context.Countries.Update(entity);
        return entity;
    }

    public async Task<Country> Delete(Country entity)
    {
        context.Countries.Remove(entity);
        return entity;
    }
}
