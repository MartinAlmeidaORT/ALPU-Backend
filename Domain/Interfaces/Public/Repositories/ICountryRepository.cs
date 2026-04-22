using Domain.Models;

namespace Domain.Interfaces.Public.Repositories;

public interface ICountryRepository
{
    public IQueryable<Country> GetAllCountries();

    public Task<Country?> GetByCodeAsync(string code);
}
