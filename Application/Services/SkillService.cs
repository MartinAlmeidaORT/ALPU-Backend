using Domain.Interfaces.Public.Repositories;
using Domain.Interfaces.Public.Services;
using Domain.Models;

namespace Application.Services;

public class SkillService(IUnitOfWork unitOfWork) : ISkillService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public IQueryable<Skill> GetAllSkills() => _unitOfWork.Skills.GetAllSkills();
}
