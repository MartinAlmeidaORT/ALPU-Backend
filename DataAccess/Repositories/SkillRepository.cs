using DataAccess.EntityFramework;
using Domain.Common.Abstracts;
using Domain.Interfaces.Public.Repositories;
using Domain.Models;

namespace DataAccess.Repositories;

public class SkillRepository(DatabaseContext context) : RepositoryBase<Skill>(context), ISkillRepository
{
    public IQueryable<Skill> GetAllSkills() => GetAll();
}
