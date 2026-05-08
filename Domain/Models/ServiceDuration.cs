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

        decimal basePrice = servicePrice.Price;

        if (input.Options.Pieces > 0 && servicePrice.VariantPrice != null)
        {
            basePrice += (decimal)(servicePrice.VariantPrice * input.Options.Pieces);
        }

        decimal discountAmount = 0m;
        foreach (var discount in VolumeDiscounts)
        {
            if (input.Options.Pieces >= discount.MinQuantity)
            {
                discountAmount = discount.Discount * basePrice;
            }
        }
        if (input.Options.IsInterior == true)
        {
            discountAmount += basePrice * 0.3m;
        }

        return Result<ServicePricePayload, AppError>.Success(new ServicePricePayload
        {
            Service = Name,
            PieceName = input.PieceName,
            Variants = input.Options.Pieces ?? 0,
            Price = basePrice,
            Discount = discountAmount,
            DurationId = input.Options.DurationId,
            TotalPriceWithDiscount = basePrice - discountAmount,
            ServiceFlags = input.Options
        });
    }
}
