using Domain.Models;
using HotChocolate.Data.Filters;

namespace GraphQL.Types.Filters;

public class BroadcasterFilterInputType : FilterInputType<Broadcaster>
{
    protected override void Configure(IFilterInputTypeDescriptor<Broadcaster> descriptor)
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Field(b => b.UserId);
        descriptor.Field(b => b.Email);
        descriptor.Field(b => b.FirstName);
        descriptor.Field(b => b.LastName);
        descriptor.Field(b => b.RUT);
        descriptor.Field(b => b.UserState);
        descriptor.Field(b => b.CategoryId);
        descriptor.Field(b => b.Skills);
        descriptor.Field(b => b.Languages);
    }
}