using Domain.Models;

namespace Domain.Interfaces.Public.Services;

public interface ISkillService
{
    IQueryable<Skill> GetAllSkills();
}
