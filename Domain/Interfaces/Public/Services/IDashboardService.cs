using Domain.Common.Payloads;

namespace Domain.Interfaces.Public.Services;

public interface IDashboardService
{
    Task<DashboardPayload> GetDashboardDataAsync();
}
