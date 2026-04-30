using Domain.Common;
using Domain.Common.Errors;

namespace Domain.Models;

public partial class ServiceDuration : Service
{
    public virtual ICollection<ServicePrice> ServicePrices { get; set; } = [];


    private Result<decimal, AppError> GetTotalPrice(bool discInterior, int pieces, int serviceId, int durationId)
    {
        ServicePrice? servicePrice = ServicePrices.FirstOrDefault(sp => sp.ServiceId == serviceId && sp.DurationId == durationId);

        if (servicePrice == null)
        {
            return Result<decimal, AppError>.Failure(AppError.NotFound("Service price not found"));
        }

        decimal totalPrice = servicePrice.Price;
        decimal discountAmmount = 0m;
        foreach (var discount in VolumeDiscounts)
        {
            if (pieces >= discount.MinQuantity)
            {
                discountAmmount = discount.Discount * totalPrice;
            }
        }
        totalPrice -= discountAmmount;
        if (discInterior)
        {
            totalPrice -= totalPrice * 0.7m;
        }

        return Result<decimal, AppError>.Success(totalPrice);
    }
}
