using DataAccess.EntityFramework;
using Domain.Classes.Abstracts;
using Domain.Interfaces.Public.Repositories;
using Domain.Models;

namespace DataAccess.Repositories;

public class CountryRepository(DatabaseContext context) : RepositoryBase<Country>(context), ICountryRepository
{
    public IQueryable<Country> GetAllCountries() => GetAll();

    public async Task<Country?> GetByCodeAsync(string code) => await Get(code);
}
