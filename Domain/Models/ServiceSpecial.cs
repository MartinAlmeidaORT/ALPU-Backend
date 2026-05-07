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
        decimal totalPrice = Price;
        decimal discountAmount = 0;
        if (input.Options.IsInterior == true)
        {
            discountAmount += totalPrice * 0.7m;
        }
        if (input.Options.HasMassMediaBroadcast == true)
        {
            discountAmount += totalPrice * 0.3m;
        }

        return Result<ServicePricePayload, AppError>.Success(new ServicePricePayload
        {
            PieceName = input.PieceName,
            Variants = null,
            Service = Name,
            Price = Price,
            Discount = discountAmount,
            TotalPriceWithDiscount = totalPrice - discountAmount,
        });
    }
}
