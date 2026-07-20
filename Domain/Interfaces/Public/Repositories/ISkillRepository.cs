using Domain.Models;

namespace Domain.Interfaces.Public.Repositories;

public interface ISkillRepository
{
    public IQueryable<Skill> GetAllSkills();
}
