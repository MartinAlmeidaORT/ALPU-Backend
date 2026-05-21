using Domain.Models.Campaign;

namespace GraphQL.Types.Objects;

public class CampaignType : ObjectType<Campaign>
{
    protected override void Configure(IObjectTypeDescriptor<Campaign> descriptor)
    {
        descriptor.Name("Campaign");
        descriptor.BindFieldsExplicitly();
        descriptor.Field(x => x.Name);
        descriptor.Field(x => x.Contract);
        descriptor.Field(x => x.Services);
    }
}
