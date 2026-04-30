using Domain.Common;
using Domain.Common.Errors;

namespace Domain.Models;

public partial class ServiceNarrative : Service
{
    public decimal BasePrice { get; set; }

    public decimal ExtraPrice { get; set; }

    public decimal RolPrice { get; set; }

    private Result<decimal, AppError> GetTotalPrice(bool discInterior, int minutes, int roles, bool nonComercial, bool internet, bool lypSync)
    {
        decimal totalPrice = BasePrice;
        if (minutes > 3) 
        {
            totalPrice += minutes * ExtraPrice;
        }
        if (discInterior)
        {
            totalPrice -= totalPrice * 0.7m;
        }
        if (nonComercial)
        {
            totalPrice -= totalPrice * 0.2m;
        }
        if (internet)
        {
            totalPrice += totalPrice * 2m;
        }
        if (lypSync)
        {
            totalPrice += totalPrice * 0.2m;
        }

        return Result<decimal, AppError>.Success(totalPrice);
    }
}
