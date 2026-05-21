using Domain.Common.Inputs.CampaignService;

namespace GraphQL.Types.Inputs;

public class CampaignServiceInputType : InputObjectType<CampaignServiceInput>
{
    protected override void Configure(IInputObjectTypeDescriptor<CampaignServiceInput> descriptor)
    {
        descriptor.Name("CampaignServiceInput");
    }
}
