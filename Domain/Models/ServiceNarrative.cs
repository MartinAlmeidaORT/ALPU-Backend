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
        decimal totalPrice = BasePrice;
        decimal discountAmount = totalPrice;

        int minutes = input.Options.NarrativeMinutes ?? 0;
        if (minutes > 3)
        {
            totalPrice += minutes * ExtraPrice;
        }

        decimal basePrice = totalPrice;

        if (input.Options.IsInterior == true)
        {
            discountAmount += totalPrice * 0.7m;
        }
        if (input.Options.IsNonComercial == true)
        {
            discountAmount += totalPrice * 0.2m;
        }
        if (input.Options.HasInternetPromo == true)
        {
            discountAmount += totalPrice * 2m;
        }
        if (input.Options.HasLipSync == true)
        {
            discountAmount += totalPrice * 0.2m;
        }

        return Result<ServicePricePayload, AppError>.Success(new ServicePricePayload
        {
            Service = this,
            Price = basePrice,
            Discount = discountAmount,
            TotalPriceWithDiscount = totalPrice - discountAmount,
        });
    }
}
