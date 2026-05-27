namespace Domain.Interfaces.Public.Services;

public interface INotificationService
{
    void NotifyContractChangeAsync(int contractId, string message);
}
