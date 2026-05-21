using Domain.Common.Inputs.CampaignService;

namespace GraphQL.Types.Inputs;

public class CampaignInputType : InputObjectType<CampaignInput>
{
    protected override void Configure(IInputObjectTypeDescriptor<CampaignInput> descriptor)
    {
        descriptor.Name("CampaignInput");
    }
}
