using Domain.Models;

namespace Application.Interfaces.Public.Services;

public interface ISkillService
{
    IQueryable<Skill> GetAllSkills();
}
