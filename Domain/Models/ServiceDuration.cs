using Domain.Common;
using Domain.Common.Errors;
using Domain.Common.Inputs;

namespace Domain.Models;

public partial class ServiceDuration : Service
{
    public virtual ICollection<ServicePrice> ServicePrices { get; set; } = [];

    public override Result<decimal, AppError> GetTotalPrice(CalculateContractServiceInput input)
    {
        ServicePrice? servicePrice = ServicePrices.FirstOrDefault(sp => sp.ServiceId == input.ServiceId && sp.DurationId == input.Options.DurationId);

        if (servicePrice == null)
        {
            return Result<decimal, AppError>.Failure(AppError.NotFound("Service price not found"));
        }

        decimal totalPrice = servicePrice.Price;
        decimal discountAmmount = 0m;
        foreach (var discount in VolumeDiscounts)
        {
            if (input.Options.Pieces >= discount.MinQuantity)
            {
                discountAmmount = discount.Discount * totalPrice;
            }
        }
        totalPrice -= discountAmmount;
        if (input.Options.IsInterior == true)
        {
            totalPrice -= totalPrice * 0.7m;
        }

        return Result<decimal, AppError>.Success(totalPrice);
    }
}
