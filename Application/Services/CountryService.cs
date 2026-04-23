using Application.DTOs.Country;
using Application.Interfaces.Public.Services;
using Application.Mappers;
using Domain.Interfaces.Public.Repositories;

namespace Application.Services;

public class CountryService(IUnitOfWork unitOfWork) : ICountryService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public IQueryable<ResultCountryDTO> GetAllCountries() => _unitOfWork.Countries.GetAllCountries().Select(CountryMapper.ToDTOExpression());
}
