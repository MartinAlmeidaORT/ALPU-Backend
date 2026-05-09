using Domain.Common;
using Domain.Common.Errors;
using Domain.Common.Inputs;
using Domain.Common.Payloads;

namespace Domain.Models;

public partial class ServiceIVR : Service
{
    public decimal InitialMessagePrice { get; set; }

    public decimal AdditionalMessagePrice { get; set; }

    public decimal UpdateMessagePrice { get; set; }

    public virtual ICollection<RangeIVR> RangeIVR { get; set; } = [];

    public override Result<ServicePricePayload, AppError> GetTotalPrice(CalculateContractServiceInput input)
    {
        decimal basePrice;

        string message = input.Options.MessageIVR ?? "";
        int additionalMessages = input.Options.AdditionalMessageIVR ?? 0;

        if (input.Options.OverridePrice != null && input.Options.OverridePrice.Value > 0)
        {
            basePrice = input.Options.OverridePrice.Value;
        }
        else
        {
            basePrice = InitialMessagePrice;
            if (additionalMessages > 0)
            {
                basePrice += additionalMessages * AdditionalMessagePrice;
            }

            int wordCount = CountWords(message);
            if (wordCount <= 100)
            {
                basePrice += 21m * wordCount;
            }
            else if (wordCount <= 200)
            {
                basePrice += 19m * wordCount;
            }
            else
            {
                basePrice += 17m * wordCount;
            }
        }

        decimal discountAmount = 0m;
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
            ServiceFlags = [
                new (input.Options.OverridePrice != null && input.Options.OverridePrice > 0, "Precio negociado"),
                new (input.Options.IsInterior ?? false, "En interior")
            ]
        });
    }

    private int CountWords(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return 0;
        }

        return text.Split([' ', '\t', '\n', '\r'], StringSplitOptions.RemoveEmptyEntries).Length;
    }
}
