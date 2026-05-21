namespace Domain.Models.Services;

public class PeriodService : BaseService
{
    public ICollection<Period> Periods { get; set; } = [];
}
