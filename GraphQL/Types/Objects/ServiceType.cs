using Domain.Models;
using Domain.Models.Services;

namespace GraphQL.Types.Objects;

public class ServiceInterfaceType : InterfaceType<BaseService>
{
    protected override void Configure(IInterfaceTypeDescriptor<BaseService> descriptor)
    {
        descriptor.Name("Service");
        descriptor.BindFieldsExplicitly();
        descriptor.Field(x => x.ServiceId);
        descriptor.Field(x => x.Name);
    }
}

public class ServiceIVRType : ObjectType<IvrService>
{
    protected override void Configure(IObjectTypeDescriptor<IvrService> descriptor)
    {
        descriptor.Name("ServiceIVR");
        descriptor.BindFieldsExplicitly();
        descriptor.Field(x => x.ServiceId);
        descriptor.Field(x => x.Name);
        descriptor.Field(x => x.InitialMessagePrice);
        descriptor.Field(x => x.AdditionalMessagePrice);
        descriptor.Field(x => x.UpdateMessagePrice);
        descriptor.Field(x => x.RangeIvr);
    }
}
