using Domain.Common;
using Domain.Common.Errors;
using Domain.Common.Inputs;
using Domain.Common.Payloads;

namespace Domain.Models;

public partial class ServiceDuration : Service
{
    public virtual ICollection<ServicePrice> ServicePrices { get; set; } = [];

    public override Result<ServicePricePayload, AppError> GetTotalPrice(CalculateContractServiceInput input)
    {
        ServicePrice? servicePrice = ServicePrices.FirstOrDefault(sp => sp.ServiceId == input.ServiceId && sp.DurationId == input.Options.DurationId);

        if (servicePrice == null)
        {
            return Result<ServicePricePayload, AppError>.Failure(AppError.NotFound("Service price not found"));
        }

        decimal totalPrice = servicePrice.Price;
        decimal discountAmount = 0m;
        foreach (var discount in VolumeDiscounts)
        {
            if (input.Options.Pieces >= discount.MinQuantity)
            {
                discountAmount = discount.Discount * totalPrice;
            }
        }
        totalPrice -= discountAmount;
        if (input.Options.IsInterior == true)
        {
            totalPrice -= totalPrice * 0.7m;
        }

        return Result<ServicePricePayload, AppError>.Success(new ServicePricePayload
        {
            Service = this,
            Price = servicePrice.Price,
            Discount = discountAmount,
            TotalPriceWithDiscount = totalPrice,
        });
    }
}
