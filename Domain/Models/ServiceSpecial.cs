using Domain.Common;
using Domain.Common.Errors;

namespace Domain.Models;

public partial class ServiceSpecial : Service
{
    public decimal Price { get; set; }

    private Result<decimal, AppError> GetTotalPrice(bool discInterior, bool? rechargeMassMedia)
    {
        decimal totalPrice = Price;
        if (discInterior)
        {
            totalPrice -= totalPrice * 0.7m;
        }
        if (rechargeMassMedia == true)
        {
            totalPrice += totalPrice * 0.3m;
        }

        return Result<decimal, AppError>.Success(totalPrice);
    }
}
