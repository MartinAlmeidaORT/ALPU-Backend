using Domain.Common;
using Domain.Common.Errors;
using Domain.Common.Inputs;

namespace Domain.Models;

public partial class ServiceIVR : Service
{
    public decimal InitialMessagePrice { get; set; }

    public decimal AdditionalMessagePrice { get; set; }

    public decimal UpdateMessagePrice { get; set; }

    public virtual ICollection<RangeIVR> RangeIVR { get; set; } = [];

    public override Result<decimal, AppError> GetTotalPrice(CalculateContractServiceInput input)
    {
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

        if (input.Options.IsInterior == true)
        {
            totalPrice -= totalPrice * 0.7m;
        }

        return Result<decimal, AppError>.Success(totalPrice);
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
