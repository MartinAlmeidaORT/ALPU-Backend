using Domain.Common;
using Domain.Common.Errors;

namespace Domain.Models;

public partial class ServiceIVR : Service
{
    public decimal InitialMessagePrice { get; set; }

    public decimal AdditionalMessagePrice { get; set; }

    public decimal UpdateMessagePrice { get; set; }

    public virtual ICollection<RangeIVR> RangeIVR { get; set; } = [];

    private Result<decimal, AppError> GetTotalPrice(bool discInterior, int additionalMessages, string message)
    {
        decimal totalPrice = InitialMessagePrice;
        
        if (additionalMessages > 0)
        {
            totalPrice += additionalMessages * AdditionalMessagePrice;
        }

        int wordCount = CountWords(message);
        if (wordCount <= 100)
        {
            totalPrice += 21m * wordCount;
        } else if (wordCount <= 200)
        {
            totalPrice += 19m * wordCount;
        } else {
            totalPrice += 17m * wordCount;
        }

        if (discInterior)
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

        return text.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
    }
}
