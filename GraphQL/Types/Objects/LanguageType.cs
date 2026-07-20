using Domain.Models;

namespace GraphQL.Types.Objects;

public class LanguageType : ObjectType<Language>
{
    protected override void Configure(IObjectTypeDescriptor<Language> descriptor)
    {
        descriptor.Name("Language");
        descriptor.BindFieldsExplicitly();
        descriptor.Field(x => x.LanguageId);
        descriptor.Field(x => x.Name);
    }
}