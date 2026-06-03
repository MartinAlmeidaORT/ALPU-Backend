using Domain.Models;

namespace GraphQL.Types.Objects;

public class BillType : ObjectType<Bill>
{
    protected override void Configure(IObjectTypeDescriptor<Bill> descriptor)
    {
        descriptor.Name("Bill");
        descriptor.BindFieldsExplicitly();
        descriptor.Field(b => b.BillId);
        descriptor.Field(b => b.Title);
        descriptor.Field(b => b.Description);
        descriptor.Field(b => b.Date);
        descriptor.Field(b => b.Amount);
        descriptor.Field(b => b.Type);
        descriptor.Field(b => b.Contract);
        descriptor.Field(b => b.ProofFile);
    }
}
