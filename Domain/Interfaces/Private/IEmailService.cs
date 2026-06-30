namespace Domain.Interfaces.Private;

public interface IEmailService
{
    Task SendAccountPendingAsync(string toEmail, string userName);
    Task SendAccountApprovedAsync(string toEmail, string userName);
}
