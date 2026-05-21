using Domain.Models.Campaign;
using Domain.Models.Campaign.Period;

namespace GraphQL.Types.Objects;

public class CampaignServiceInterface : InterfaceType<BaseCampaignService>
{
    protected override void Configure(IInterfaceTypeDescriptor<BaseCampaignService> descriptor)
    {
        descriptor.Name("BaseCampaignService");
        descriptor.BindFieldsExplicitly();
        descriptor.Field(x => x.Campaign);
        descriptor.Field(x => x.Pieces);
        descriptor.Field(x => x.BasePriceOverride);
    }
}

public class EventCampaignServiceType : ObjectType<EventCampaignService>
{
    protected override void Configure(IObjectTypeDescriptor<EventCampaignService> descriptor)
    {
        descriptor.Name("EventCampaignService");
        descriptor.BindFieldsExplicitly();
        descriptor.Implements<CampaignServiceInterface>();
        descriptor.Field(x => x.Campaign);
        descriptor.Field(x => x.Pieces);
        descriptor.Field(x => x.BasePriceOverride);
        descriptor.Field(x => x.Date);
    }
}

public class NarrativeCampaignServiceType : ObjectType<NarrativeCampaignService>
{
    protected override void Configure(IObjectTypeDescriptor<NarrativeCampaignService> descriptor)
    {
        descriptor.Name("NarrativeCampaignService");
        descriptor.BindFieldsExplicitly();
        descriptor.Implements<CampaignServiceInterface>();
        descriptor.Field(x => x.Campaign);
        descriptor.Field(x => x.Pieces);
        descriptor.Field(x => x.BasePriceOverride);
    }
}

public class IvrCampaignServiceType : ObjectType<IvrCampaignService>
{
    protected override void Configure(IObjectTypeDescriptor<IvrCampaignService> descriptor)
    {
        descriptor.Name("IvrCampaignService");
        descriptor.BindFieldsExplicitly();
        descriptor.Implements<CampaignServiceInterface>();
        descriptor.Field(x => x.Campaign);
        descriptor.Field(x => x.Pieces);
        descriptor.Field(x => x.BasePriceOverride);
    }
}

public class PeriodCampaignServiceInterface : InterfaceType<PeriodCampaignService>
{
    protected override void Configure(IInterfaceTypeDescriptor<PeriodCampaignService> descriptor)
    {
        descriptor.Name("PeriodCampaignService");
        descriptor.BindFieldsExplicitly();
        descriptor.Implements<CampaignServiceInterface>();
        descriptor.Field(x => x.Campaign);
        descriptor.Field(x => x.Pieces);
        descriptor.Field(x => x.BasePriceOverride);
    }
}

public class TvCampaignServiceType : ObjectType<TvCampaignService>
{
    protected override void Configure(IObjectTypeDescriptor<TvCampaignService> descriptor)
    {
        descriptor.Name("TvCampaignService");
        descriptor.BindFieldsExplicitly();
        descriptor.Implements<PeriodCampaignServiceInterface>();
        descriptor.Field(x => x.Campaign);
        descriptor.Field(x => x.Pieces);
        descriptor.Field(x => x.BasePriceOverride);
    }
}

public class RadioCampaignServiceType : ObjectType<RadioCampaignService>
{
    protected override void Configure(IObjectTypeDescriptor<RadioCampaignService> descriptor)
    {
        descriptor.Name("RadioCampaignService");
        descriptor.BindFieldsExplicitly();
        descriptor.Implements<PeriodCampaignServiceInterface>();
        descriptor.Field(x => x.Campaign);
        descriptor.Field(x => x.Pieces);
        descriptor.Field(x => x.BasePriceOverride);
    }
}

public class CinemaCampaignServiceType : ObjectType<CinemaCampaignService>
{
    protected override void Configure(IObjectTypeDescriptor<CinemaCampaignService> descriptor)
    {
        descriptor.Name("CinemaCampaignService");
        descriptor.BindFieldsExplicitly();
        descriptor.Implements<PeriodCampaignServiceInterface>();
        descriptor.Field(x => x.Campaign);
        descriptor.Field(x => x.Pieces);
        descriptor.Field(x => x.BasePriceOverride);
    }
}

public class CameraCampaignServiceType : ObjectType<CameraCampaignService>
{
    protected override void Configure(IObjectTypeDescriptor<CameraCampaignService> descriptor)
    {
        descriptor.Name("CameraCampaignService");
        descriptor.BindFieldsExplicitly();
        descriptor.Implements<PeriodCampaignServiceInterface>();
        descriptor.Field(x => x.Campaign);
        descriptor.Field(x => x.Pieces);
        descriptor.Field(x => x.BasePriceOverride);
    }
}

public class OtherMediaCampaignServiceType : ObjectType<OtherMediaCampaignService>
{
    protected override void Configure(IObjectTypeDescriptor<OtherMediaCampaignService> descriptor)
    {
        descriptor.Name("OtherMediaCampaignService");
        descriptor.BindFieldsExplicitly();
        descriptor.Implements<PeriodCampaignServiceInterface>();
        descriptor.Field(x => x.Campaign);
        descriptor.Field(x => x.Pieces);
        descriptor.Field(x => x.BasePriceOverride);
    }
}
