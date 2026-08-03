using Domain.Models;

namespace Domain.Interfaces.Public.Services;

public interface ICountryService
{
    IQueryable<Country> GetAllCountries();
}
