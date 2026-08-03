using Domain.Models;

namespace Domain.Interfaces.Public.Services;

public interface ILanguageService
{
    IQueryable<Language> GetAllLanguages();
}
