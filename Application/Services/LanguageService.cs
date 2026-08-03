using Domain.Interfaces.Public.Repositories;
using Domain.Interfaces.Public.Services;
using Domain.Models;

namespace Application.Services;

public class LanguageService(IUnitOfWork unitOfWork) : ILanguageService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public IQueryable<Language> GetAllLanguages() => _unitOfWork.Languages.GetAllLanguages();
}
