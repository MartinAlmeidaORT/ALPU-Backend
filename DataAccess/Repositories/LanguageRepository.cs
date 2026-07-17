using DataAccess.EntityFramework;
using Domain.Common.Abstracts;
using Domain.Interfaces.Public.Repositories;
using Domain.Models;

namespace DataAccess.Repositories;

public class LanguageRepository(DatabaseContext context) : RepositoryBase<Language>(context), ILanguageRepository
{
    public IQueryable<Language> GetAllLanguages() => GetAll();
}
