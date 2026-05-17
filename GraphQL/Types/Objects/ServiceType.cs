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
        descriptor.Field(x => x.BasePrice);
        descriptor.Field(x => x.ExtraPrice);
        descriptor.Field(x => x.FirstExtraPrice);
        descriptor.Field(x => x.Type);
    }
}

public class ServiceIvrType : ObjectType<IvrService>
{
    protected override void Configure(IObjectTypeDescriptor<IvrService> descriptor)
    {
        descriptor.Name("ServiceIvr");
        descriptor.BindFieldsExplicitly();
        descriptor.Implements<ServiceInterfaceType>();
        descriptor.Field(x => x.ServiceId);
        descriptor.Field(x => x.Name);
        descriptor.Field(x => x.BasePrice);
        descriptor.Field(x => x.ExtraPrice);
        descriptor.Field(x => x.FirstExtraPrice);
        descriptor.Field(x => x.Type);
        descriptor.Field(x => x.InitialMessagePrice);
        descriptor.Field(x => x.AdditionalMessagePrice);
        descriptor.Field(x => x.UpdateMessagePrice);
        descriptor.Field(x => x.RangeIvr);
    }
}

public class ServicePeriodType : ObjectType<PeriodService>
{
    protected override void Configure(IObjectTypeDescriptor<PeriodService> descriptor)
    {
        descriptor.Name("ServicePeriod");
        descriptor.BindFieldsExplicitly();
        descriptor.Implements<ServiceInterfaceType>();
        descriptor.Field(x => x.ServiceId);
        descriptor.Field(x => x.Name);
        descriptor.Field(x => x.BasePrice);
        descriptor.Field(x => x.ExtraPrice);
        descriptor.Field(x => x.FirstExtraPrice);
        descriptor.Field(x => x.Type);
        descriptor.Field(x => x.Periods).UseSorting();
    }
}

public class ServiceNarrativeType : ObjectType<NarrativeService>
{
    protected override void Configure(IObjectTypeDescriptor<NarrativeService> descriptor)
    {
        descriptor.Name("ServiceNarrative");
        descriptor.BindFieldsExplicitly();
        descriptor.Implements<ServiceInterfaceType>();
        descriptor.Field(x => x.ServiceId);
        descriptor.Field(x => x.Name);
        descriptor.Field(x => x.BasePrice);
        descriptor.Field(x => x.ExtraPrice);
        descriptor.Field(x => x.FirstExtraPrice);
        descriptor.Field(x => x.Type);
        descriptor.Field(x => x.RolePrice);
    }
}

public class ServiceDateType : ObjectType<DateService>
{
    protected override void Configure(IObjectTypeDescriptor<DateService> descriptor)
    {
        descriptor.Name("ServiceDate");
        descriptor.BindFieldsExplicitly();
        descriptor.Implements<ServiceInterfaceType>();
        descriptor.Field(x => x.ServiceId);
        descriptor.Field(x => x.Name);
        descriptor.Field(x => x.BasePrice);
        descriptor.Field(x => x.ExtraPrice);
        descriptor.Field(x => x.FirstExtraPrice);
        descriptor.Field(x => x.Type);
    }
}
