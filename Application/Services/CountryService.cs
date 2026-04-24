using Application.Interfaces.Public.Services;
using Application.Mappers;
using Domain.Interfaces.Public.Repositories;
using Domain.Models;

namespace Application.Services;

public class CountryService(IUnitOfWork unitOfWork) : ICountryService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public IQueryable<Country> GetAllCountries() => _unitOfWork.Countries.GetAllCountries();
}
