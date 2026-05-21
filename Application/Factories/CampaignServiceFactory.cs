using Application.Common.Extensions;
using Domain.Common.Inputs.CampaignService;
using Domain.Enums;
using Domain.Interfaces.Private;
using Domain.Interfaces.Public.Singletons;
using Domain.Models.Campaign;
using Domain.Models.Campaign.Period;
using Domain.Models.Services;
using FluentResults;

namespace Application.Factories;

public class CampaignServiceFactory(IPriceTable alpuService) : ICampaignServiceFactory
{
    private readonly IPriceTable _priceTable = alpuService;

    public async Task<Result<BaseCampaignService>> Create(CampaignServiceInput request)
    {
        BaseService? service = await _priceTable.GetServiceById(request.ServiceId);
        if (service == null) return Result.Fail($"Servicio no encontrado: id={request.ServiceId}");

        Result<List<Piece>> pieces = BuildPieces(request.Pieces);
        if (pieces.IsFailed) return Result.Fail(pieces.Errors);

        return service.Type switch
        {
            ServiceType.TvGeneric => BuildTv(request, service, pieces.Value),
            ServiceType.TvZocalo => BuildTv(request, service, pieces.Value),
            ServiceType.TvHost => BuildTv(request, service, pieces.Value),
            ServiceType.RadioGeneric => BuildRadio(request, service, pieces.Value),
            ServiceType.RadioZocalo => BuildRadio(request, service, pieces.Value),
            ServiceType.RadioHost => BuildRadio(request, service, pieces.Value),
            ServiceType.InternetVideo => BuildInternet(request, service, pieces.Value),
            ServiceType.InternetAudio => BuildInternet(request, service, pieces.Value),
            ServiceType.Cinema => BuildCinema(request, service, pieces.Value),
            ServiceType.Camera => BuildCamera(request, service, pieces.Value),
            ServiceType.Narrative => BuildNarrative(request, service, pieces.Value),
            ServiceType.Ivr => BuildIvr(request, service, pieces.Value),
            ServiceType.Event => BuildEvent(request, service, pieces.Value),
            ServiceType.OthersVideo => BuildOtherMedia(request, service, pieces.Value),
            ServiceType.OthersAudio => BuildOtherMedia(request, service, pieces.Value),
            ServiceType.Others => BuildOtherMedia(request, service, pieces.Value),
            _ => Result.Fail($"Tipo de servicio no soportado: {service.Type}")
        };
    }

    public static Result<List<Piece>> BuildPieces(List<PieceInput> pieces)
    {
        if (pieces.Count == 0 || string.IsNullOrWhiteSpace(pieces[0].Name))
            return Result.Fail("La primera pieza debe tener un nombre.");

        string basePieceName = pieces[0].Name!;

        return pieces.Select((p, i) => new Piece
        {
            Name = string.IsNullOrWhiteSpace(p.Name) ? $"{basePieceName}#{i + 1}" : p.Name
        }).ToList();
    }

    private Result<BaseCampaignService> BuildService<O>(
        CampaignServiceInput request,
        BaseService service,
        List<Piece> pieces,
        Func<BaseService, List<Piece>, IPriceTable, O, BaseCampaignService> factory)
    {
        Result<O> options = request.Options.ConvertJsonElementToRecord<O>();
        if (options.IsSuccess)
        {
            return factory(service, pieces, _priceTable, options.Value);
        }
        else
        {
            return Result.Fail(options.Errors);
        }
    }

    private Result<BaseCampaignService> BuildTv(CampaignServiceInput request, BaseService service, List<Piece> pieces)
    {
        return BuildService<TvCampaignServiceOptions>(request, service, pieces, (s, p, pT, o) => new TvCampaignService((PeriodService)s, p, pT, o));
    }

    private Result<BaseCampaignService> BuildRadio(CampaignServiceInput request, BaseService service, List<Piece> pieces)
    {
        return BuildService<RadioCampaignServiceOptions>(request, service, pieces, (s, p, pT, o) => new RadioCampaignService((PeriodService)s, p, pT, o));
    }

    private Result<BaseCampaignService> BuildInternet(CampaignServiceInput request, BaseService service, List<Piece> pieces)
    {
        return BuildService<InternetCampaignServiceOptions>(request, service, pieces, (s, p, pT, o) => new InternetCampaignService((PeriodService)s, p, pT, o));
    }

    private Result<BaseCampaignService> BuildCinema(CampaignServiceInput request, BaseService service, List<Piece> pieces)
    {
        return BuildService<CinemaCampaignServiceOptions>(request, service, pieces, (s, p, pT, o) => new CinemaCampaignService((PeriodService)s, p, pT, o));
    }

    private Result<BaseCampaignService> BuildCamera(CampaignServiceInput request, BaseService service, List<Piece> pieces)
    {
        return BuildService<CameraCampaignServiceOptions>(request, service, pieces, (s, p, pT, o) => new CameraCampaignService((PeriodService)s, p, pT, o));
    }

    private Result<BaseCampaignService> BuildNarrative(CampaignServiceInput request, BaseService service, List<Piece> pieces)
    {
        return BuildService<NarrativeCampaignServiceOptions>(request, service, pieces, (s, p, pT, o) => new NarrativeCampaignService((NarrativeService)s, p, pT, o));
    }

    private Result<BaseCampaignService> BuildIvr(CampaignServiceInput request, BaseService service, List<Piece> pieces)
    {
        return BuildService<IvrCampaignServiceOptions>(request, service, pieces, (s, p, pT, o) => new IvrCampaignService((IvrService)s, p, pT, o));
    }

    private Result<BaseCampaignService> BuildEvent(CampaignServiceInput request, BaseService service, List<Piece> pieces)
    {
        return BuildService<EventCampaignServiceOptions>(request, service, pieces, (s, p, pT, o) => new EventCampaignService(s, p, pT, o));
    }

    private Result<BaseCampaignService> BuildOtherMedia(CampaignServiceInput request, BaseService service, List<Piece> pieces)
    {
        return BuildService<OtherMediaCampaignServiceOptions>(request, service, pieces, (s, p, pT, o) => new OtherMediaCampaignService((PeriodService)s, p, pT, o));
    }
}
