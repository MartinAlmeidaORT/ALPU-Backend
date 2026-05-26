using Domain.Enums;

namespace Domain.Models.Services;

public class Period
{
    internal Period() { }

    public int ServiceId { get; set; }

    public PeriodService Service { get; set; } = null!;

    public decimal BasePrice { get; set; }

    public decimal? ExtraPrice { get; set; }

    public decimal? FirstExtraPrice { get; set; }

    public Interval Interval { get; set; }
}
