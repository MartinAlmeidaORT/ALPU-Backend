using Domain.Common.Inputs.CampaignService;
using Domain.Common.Payloads;
using Domain.Enums;
using Domain.Interfaces.Public.Singletons;
using Domain.Models.Services;
using FluentResults;

namespace Domain.Models.Campaign;

public abstract class BaseCampaignService
{
    internal protected BaseCampaignService()
    {
        _priceTable = null!;
    }

    protected BaseCampaignService(BaseService service, List<Piece> pieces, IPriceTable priceTable)
    {
        Service = service;
        Pieces = pieces;
        _priceTable = priceTable;
    }

    protected readonly IPriceTable _priceTable;

    protected BaseCampaignServiceOptions Options { get; set; } = null!;

    public int CampaignServiceId { get; set; }

    public int CampaignId { get; set; }

    public Campaign Campaign { get; set; } = null!;

    public int ServiceId { get; set; }

    public BaseService Service { get; set; } = null!;

    public List<Piece> Pieces { get; init; } = null!;

    public decimal? BasePriceOverride { get; set; }

    public virtual async Task<Result<ServiceBreakdown>> Calculate(CampaignInput campaign)
    {
        if (Service.BasePrice == null) throw new NullReferenceException();

        ServiceBreakdown breakdown = new()
        {
            ServiceName = Service.Name,
            ServiceType = Service.Type,
            BasePrice = (decimal)Service.BasePrice,
            SubsequentPrice = Service.ExtraPrice,
            Pieces = CalculatePieces(),
            BeforeDiscount = CalculateSubTotal()
        };

        decimal subtotal = breakdown.BeforeDiscount;

        VolumeDiscount? volumeDiscount = await _priceTable.GetVolumeDiscountAsync(Service.Type, Pieces.Count);
        if (volumeDiscount != null)
        {
            ApplyVolumeDiscount(ref breakdown, volumeDiscount);
        }

        breakdown.SubTotal = subtotal;
        return breakdown;
    }

    public virtual decimal CalculateSubTotal()
    {
        if (Service.BasePrice == null) throw new NullReferenceException();

        decimal total = (decimal)Service.BasePrice;
        int pieces = Pieces.Count - 1;

        if (pieces > 0 && Service.FirstExtraPrice != null)
        {
            total += (decimal)Service.FirstExtraPrice;
            pieces--;
        }

        if (pieces > 0 && Service.ExtraPrice != null)
        {
            total += (decimal)Service.ExtraPrice * pieces;
        }
        else if (pieces > 0)
        {
            total += (decimal)Service.BasePrice * pieces;
        }

        return total;
    }

    public void ApplyVolumeDiscount(ref ServiceBreakdown breakdown, VolumeDiscount volumeDiscount)
    {
        decimal adjusted = volumeDiscount.Type switch
        {
            PriceAdjustmentType.Percentage => breakdown.SubTotal * volumeDiscount.Amount,
            PriceAdjustmentType.Fixed => breakdown.SubTotal + volumeDiscount.Amount,
            _ => throw new ArgumentException()
        };

        decimal difference = adjusted - breakdown.SubTotal;
        breakdown.Adjustments.Add(new("volume_discount", volumeDiscount.Name, volumeDiscount.Amount, difference, volumeDiscount.Type));
        breakdown.SubTotal += difference;
    }

    public PieceBreakdown[] CalculatePieces()
    {
        if (Service.BasePrice == null) throw new NullReferenceException();

        int pieces = Pieces.Count;
        PieceBreakdown[] pieceBreakdowns = new PieceBreakdown[pieces];

        for (int i = 0; i < Pieces.Count; i++)
        {
            if (i == 0)
            {
                pieceBreakdowns[i] = new()
                {
                    Name = Pieces[i].Name,
                    IsSubsequent = false,
                    Price = (decimal)Service.BasePrice,
                };
            }
            else if (i == 1 && Service.FirstExtraPrice != null)
            {
                pieceBreakdowns[i] = new()
                {
                    Name = Pieces[i].Name,
                    IsSubsequent = true,
                    Price = (decimal)Service.FirstExtraPrice,
                };
            }
            else
            {
                pieceBreakdowns[i] = new()
                {
                    Name = Pieces[i].Name,
                    IsSubsequent = true,
                    Price = Service.ExtraPrice ?? (decimal)Service.BasePrice,
                };
            }
        }
        return pieceBreakdowns;
    }

    public abstract DateOnly GetExpireDate();
}
