using Domain.Common.Errors;
using Domain.Common.Inputs;
using Domain.Common.Payloads;
using FluentResults;

namespace Domain.Models;

public partial class ServiceDuration : Service
{
    public virtual ICollection<ServicePrice> ServicePrices { get; set; } = [];

    public override Result<ServicePricePayload> GetTotalPrice(CalculateContractServiceInput input)
    {
        ServicePrice? servicePrice = ServicePrices.FirstOrDefault(sp => sp.ServiceId == input.ServiceId && sp.DurationId == input.Options.DurationId);

        if (servicePrice == null)
        {
            return new NotFoundError("Service price not found");
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

        return new ServicePricePayload
        {
            Service = Name,
            PieceName = input.PieceName,
            Variants = input.Options.Pieces ?? 0,
            Price = basePrice,
            Discount = discountAmount,
            TotalPriceWithDiscount = basePrice - discountAmount,
            ServiceFlags = [
                new (input.Options.OverridePrice != null && input.Options.OverridePrice > 0, "Precio negociado"),
                new (input.Options.IsInterior ?? false, "En interior")
            ]
        };
    }
}
