using Domain.Common;
using Domain.Common.Errors;
using Domain.Common.Inputs;
using Domain.Common.Payloads;

namespace Domain.Models;

public abstract class Service : Entity
{
    public int ServiceId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Piece> Pieces { get; set; } = [];

    public virtual ICollection<VolumeDiscount> VolumeDiscounts { get; set; } = [];

    public abstract Result<ServicePricePayload, AppError> GetTotalPrice(CalculateContractServiceInput input);
}
