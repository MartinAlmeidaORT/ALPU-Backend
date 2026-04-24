using Domain.Models;

namespace Application.Interfaces.Public.Services;

public interface ICountryService
{
    IQueryable<Country> GetAllCountries();
}
