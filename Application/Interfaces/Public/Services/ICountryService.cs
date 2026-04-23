using Application.DTOs.Country;

namespace Application.Interfaces.Public.Services;

public interface ICountryService
{
    IQueryable<ResultCountryDTO> GetAllCountries();
}
