using Domain.Common;
using Domain.Common.Errors;
using Domain.Common.Inputs;

namespace Domain.Models;

public partial class ServiceSpecial : Service
{
    public decimal Price { get; set; }

    public override Result<decimal, AppError> GetTotalPrice(CalculateContractServiceInput input)
    {
        decimal totalPrice = Price;
        if (input.Options.IsInterior == true)
        {
            totalPrice -= totalPrice * 0.7m;
        }
        if (input.Options.HasMassMediaBroadcast == true)
        {
            totalPrice += totalPrice * 0.3m;
        }

        return Result<decimal, AppError>.Success(totalPrice);
    }
}
