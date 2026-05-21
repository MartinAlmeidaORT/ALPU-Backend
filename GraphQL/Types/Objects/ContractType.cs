using Domain.Models;

namespace GraphQL.Types.Objects;

public class ContractType : ObjectType<Contract>
{
    protected override void Configure(IObjectTypeDescriptor<Contract> descriptor)
    {
        descriptor.Name("Contract");
        descriptor.BindFieldsExplicitly();
        descriptor.Field(x => x.ContractId);
        descriptor.Field(x => x.Client);
        descriptor.Field(x => x.Broadcaster);
        descriptor.Field(x => x.CountryCode);
        descriptor.Field(x => x.Date);
        descriptor.Field(x => x.DueDate);
        descriptor.Field(x => x.Campaigns);
        descriptor.Field(x => x.Country);
        descriptor.Field(x => x.State);
        descriptor.Field(x => x.ClientApproved);
        descriptor.Field(x => x.BroadcasterApproved);
    }
}
