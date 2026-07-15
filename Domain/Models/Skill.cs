using Domain.Common;
using Domain.Common.Errors;

namespace Domain.Models;

public class Skill : Entity
{
    internal Skill() { }

    public int SkillId { get; set; }

    public string Name { get; set; } = null!;


}

public static class SkillErrors
{
    public class SkillNotFoundError(string msg) : NotFoundError(msg);

    public static SkillNotFoundError SkillNotFound(int skillId) => new($"Skill con id {skillId} no fue encontrada.");
}
