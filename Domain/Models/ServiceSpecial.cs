using Domain.Common;
using Domain.Common.Errors;
using Domain.Common.Inputs;
using Domain.Common.Payloads;

namespace Domain.Models;

public partial class ServiceSpecial : Service
{
    public decimal Price { get; set; }

    public override Result<ServicePricePayload, AppError> GetTotalPrice(CalculateContractServiceInput input)
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

        return Result<ServicePricePayload, AppError>.Success(new ServicePricePayload
        {
            PieceName = input.PieceName,
            Variants = null,
            Service = Name,
            Price = basePrice,
            Discount = discountAmount,
            TotalPriceWithDiscount = basePrice - discountAmount,
            ServiceFlags = input.Options
        });
    }
}
