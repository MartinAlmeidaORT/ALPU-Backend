using Domain.Common.Inputs;
using Domain.Common.Payloads;
using FluentResults;

namespace Domain.Models;

public partial class ServiceSpecial : Service
{
    public decimal Price { get; set; }

    public override Result<ServicePricePayload> GetTotalPrice(CalculateContractServiceInput input)
    {
        decimal basePrice = Price;
        if (input.Options.HasMassMediaBroadcast == true)
        {
            basePrice += basePrice * 1.3m;
        }
        decimal discountAmount = 0;
        if (input.Options.IsInterior == true)
        {
            discountAmount += basePrice * 0.3m;
        }

        return new ServicePricePayload
        {
            PieceName = input.PieceName,
            Variants = null,
            Service = Name,
            Price = basePrice,
            Discount = discountAmount,
            TotalPriceWithDiscount = basePrice - discountAmount,
            ServiceFlags = [
                new (input.Options.IsInterior ?? false, "En interior"),
                new (input.Options.HasMassMediaBroadcast ?? false, "Difusion en medios masivos"),
            ]
        };
    }
}
