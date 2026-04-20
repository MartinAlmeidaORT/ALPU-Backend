using Domain.Models;

namespace Domain.Interfaces.Public.Repositories;

public interface ICountryRepository : IRepository<Country>
{
    public Task<Country?> GetByCodeAsync(string code);
}
