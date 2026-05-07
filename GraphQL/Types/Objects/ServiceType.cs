using Domain.Models;

namespace GraphQL.Types.Objects;

public class ServiceInterfaceType : InterfaceType<Service>
{
    protected override void Configure(IInterfaceTypeDescriptor<Service> descriptor)
    {
        descriptor.Name("Service");
    }
}

public class ServiceDurationType : ObjectType<ServiceDuration>
{
    protected override void Configure(IObjectTypeDescriptor<ServiceDuration> descriptor)
    {
        descriptor.Name("ServiceDuration");

        descriptor
             .Field(s => s.ServicePrices)
             .UseSorting();
    }
}

public class ServiceIVRType : ObjectType<ServiceIVR>
{
    protected override void Configure(IObjectTypeDescriptor<ServiceIVR> descriptor)
    {
        descriptor.Name("ServiceIVR");
    }
}

public class ServiceNarrativeType : ObjectType<ServiceNarrative>
{
    protected override void Configure(IObjectTypeDescriptor<ServiceNarrative> descriptor)
    {
        descriptor.Name("ServiceNarrative");
    }
}

public class ServiceSpecialType : ObjectType<ServiceSpecial>
{
    protected override void Configure(IObjectTypeDescriptor<ServiceSpecial> descriptor)
    {
        descriptor.Name("ServiceSpecial");
    }
}
