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
        decimal basePrice = 0m;
        decimal discountAmount = 0m;
        decimal totalPrice = InitialMessagePrice;

        string message = input.Options.MessageIVR ?? "";
        int additionalMessages = input.Options.AdditionalMessageIVR ?? 0;

        if (additionalMessages > 0)
        {
            totalPrice += additionalMessages * AdditionalMessagePrice;
        }

        int wordCount = CountWords(message);
        if (wordCount <= 100)
        {
            totalPrice += 21m * wordCount;
        }
        else if (wordCount <= 200)
        {
            totalPrice += 19m * wordCount;
        }
        else
        {
            totalPrice += 17m * wordCount;
        }

        basePrice = totalPrice;
        if (input.Options.IsInterior == true)
        {
            discountAmount += totalPrice * 0.7m;
        }

        return Result<ServicePricePayload, AppError>.Success(new ServicePricePayload
        {
            PieceName = input.PieceName,
            Variants = null,
            Service = Name,
            Price = basePrice,
            Discount = discountAmount,
            TotalPriceWithDiscount = totalPrice - discountAmount,
            ServiceFlags = input.Options
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
