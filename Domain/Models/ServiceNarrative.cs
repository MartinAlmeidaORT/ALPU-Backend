using Domain.Common;
using Domain.Common.Errors;
using Domain.Common.Inputs;

namespace Domain.Models;

public partial class ServiceNarrative : Service
{
    public decimal BasePrice { get; set; }

    public decimal ExtraPrice { get; set; }

    public decimal RolPrice { get; set; }

    public override Result<decimal, AppError> GetTotalPrice(CalculateContractServiceInput input)
    {
        decimal totalPrice = BasePrice;
        int minutes = input.Options.NarrativeMinutes ?? 0;
        if (minutes > 3)
        {
            totalPrice += minutes * ExtraPrice;
        }
        if (input.Options.IsInterior == true)
        {
            totalPrice -= totalPrice * 0.7m;
        }
        if (input.Options.IsNonComercial == true)
        {
            totalPrice -= totalPrice * 0.2m;
        }
        if (input.Options.HasInternetPromo == true)
        {
            totalPrice += totalPrice * 2m;
        }
        if (input.Options.HasLipSync == true)
        {
            totalPrice += totalPrice * 0.2m;
        }

        return Result<decimal, AppError>.Success(totalPrice);
    }
}
