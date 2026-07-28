using Domain.Models;

namespace Application.Interfaces.Public.Services;

public interface ILanguageService
{
    IQueryable<Language> GetAllLanguages();
}
