using Domain.Common;
using Domain.Common.Errors;
using Domain.Common.Inputs;
using Domain.Common.Payloads;

namespace Domain.Models;

public partial class ServiceNarrative : Service
{
    public decimal BasePrice { get; set; }

    public decimal ExtraPrice { get; set; }

    public decimal RolPrice { get; set; }

    public override Result<ServicePricePayload, AppError> GetTotalPrice(CalculateContractServiceInput input)
    {
        decimal basePrice;

        if (input.Options.OverridePrice != null && input.Options.OverridePrice.Value > 0)
        {
            basePrice = input.Options.OverridePrice.Value;
        }
        else
        {
            basePrice = BasePrice;
            int minutes = input.Options.NarrativeMinutes ?? 0;
            if (minutes > 3)
            {
                basePrice += minutes * ExtraPrice;
            }
            if (input.Options.RoleQuantity > 0)
            {
                basePrice += RolPrice * (decimal)input.Options.RoleQuantity;
            }
            if (input.Options.HasInternetPromo == true)
            {
                basePrice += basePrice * 2m;
            }
        }

        decimal discountAmount = 0;
        if (input.Options.IsInterior == true)
        {
            discountAmount += basePrice * 0.3m;
        }
        if (input.Options.IsNonComercial == true)
        {
            discountAmount += basePrice * 0.2m;
        }
        if (input.Options.HasLipSync == true)
        {
            discountAmount += basePrice * 0.2m;
        }

        return Result<ServicePricePayload, AppError>.Success(new ServicePricePayload
        {
            PieceName = input.PieceName,
            Variants = null,
            Service = Name,
            Price = basePrice,
            Discount = discountAmount,
            TotalPriceWithDiscount = basePrice - discountAmount,
            ServiceFlags = [
                new (input.Options.OverridePrice != null && input.Options.OverridePrice > 0, "Precio negociado"),
                new (input.Options.NarrativeMinutes != null && input.Options.NarrativeMinutes > 0, $"Minutos extras: {input.Options.NarrativeMinutes}"),
                new (input.Options.IsInterior ?? false, "En interior"),
                new (input.Options.IsNonComercial ?? false, "No comercial"),
                new (input.Options.HasLipSync ?? false, "Sincro labial")
            ]
        });
    }
}
