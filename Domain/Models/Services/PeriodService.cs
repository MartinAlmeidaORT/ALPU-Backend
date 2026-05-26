namespace Domain.Models.Services;

public class PeriodService : BaseService
{
    internal PeriodService() { }

    public ICollection<Period> Periods { get; set; } = [];
}
