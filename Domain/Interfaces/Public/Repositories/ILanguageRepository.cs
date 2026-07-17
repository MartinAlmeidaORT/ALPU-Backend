using Domain.Models;

namespace Domain.Interfaces.Public.Repositories;

public interface ILanguageRepository
{
    public IQueryable<Language> GetAllLanguages();

}
