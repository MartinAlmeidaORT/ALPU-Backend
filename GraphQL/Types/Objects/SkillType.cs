using Domain.Models;

namespace GraphQL.Types.Objects;

public class SkillType : ObjectType<Skill>
{
    protected override void Configure(IObjectTypeDescriptor<Skill> descriptor)
    {
        descriptor.Name("Skill");
        descriptor.BindFieldsExplicitly();
        descriptor.Field(x => x.SkillId);
        descriptor.Field(x => x.Name);
    }
}