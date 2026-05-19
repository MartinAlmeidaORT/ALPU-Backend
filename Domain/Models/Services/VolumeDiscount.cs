using Domain.Common;
using Domain.Enums;

namespace Domain.Models.Services;

public class VolumeDiscount : Entity
{
    public int VolumeDiscountId { get; set; }

    public string Name { get; set; } = null!;

    public ServiceType ServiceType { get; set; }

    public int MinQuantity { get; set; }

    public int? MaxQuantity { get; set; }

    public PriceAdjustmentType Type { get; set; }

    public decimal Amount { get; set; }
}
