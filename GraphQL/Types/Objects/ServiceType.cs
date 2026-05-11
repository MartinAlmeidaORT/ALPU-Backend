using Domain.Models;

namespace GraphQL.Types.Objects;

public class ServiceInterfaceType : InterfaceType<Service>
{
    protected override void Configure(IInterfaceTypeDescriptor<Service> descriptor)
    {
        descriptor.Name("Service");
        descriptor.BindFieldsExplicitly();
        descriptor.Field(x => x.ServiceId);
        descriptor.Field(x => x.Name);
        descriptor.Field(x => x.Pieces);
        descriptor.Field(x => x.VolumeDiscounts);
    }
}

public class ServiceDurationType : ObjectType<ServiceDuration>
{
    protected override void Configure(IObjectTypeDescriptor<ServiceDuration> descriptor)
    {
        descriptor.Name("ServiceDuration");
        descriptor.BindFieldsExplicitly();
        descriptor.Field(x => x.ServiceId);
        descriptor.Field(x => x.Name);
        descriptor.Field(x => x.Pieces);
        descriptor.Field(x => x.VolumeDiscounts);
        descriptor.Field(x => x.ServicePrices).UseSorting();
    }
}

public class ServiceIVRType : ObjectType<ServiceIVR>
{
    protected override void Configure(IObjectTypeDescriptor<ServiceIVR> descriptor)
    {
        descriptor.Name("ServiceIVR");
        descriptor.BindFieldsExplicitly();
        descriptor.Field(x => x.ServiceId);
        descriptor.Field(x => x.Name);
        descriptor.Field(x => x.Pieces);
        descriptor.Field(x => x.VolumeDiscounts);
        descriptor.Field(x => x.InitialMessagePrice);
        descriptor.Field(x => x.AdditionalMessagePrice);
        descriptor.Field(x => x.UpdateMessagePrice);
        descriptor.Field(x => x.RangeIVR);
    }
}

public class ServiceNarrativeType : ObjectType<ServiceNarrative>
{
    protected override void Configure(IObjectTypeDescriptor<ServiceNarrative> descriptor)
    {
        descriptor.Name("ServiceNarrative");
        descriptor.BindFieldsExplicitly();
        descriptor.Field(x => x.ServiceId);
        descriptor.Field(x => x.Name);
        descriptor.Field(x => x.Pieces);
        descriptor.Field(x => x.VolumeDiscounts);
        descriptor.Field(x => x.BasePrice);
        descriptor.Field(x => x.ExtraPrice);
        descriptor.Field(x => x.RolPrice);
    }
}

public class ServiceSpecialType : ObjectType<ServiceSpecial>
{
    protected override void Configure(IObjectTypeDescriptor<ServiceSpecial> descriptor)
    {
        descriptor.Name("ServiceSpecial");
        descriptor.BindFieldsExplicitly();
        descriptor.Field(x => x.ServiceId);
        descriptor.Field(x => x.Name);
        descriptor.Field(x => x.Pieces);
        descriptor.Field(x => x.VolumeDiscounts);
        descriptor.Field(x => x.Price);
    }
}
