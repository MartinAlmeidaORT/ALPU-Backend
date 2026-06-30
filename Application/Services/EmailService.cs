using Domain.Interfaces.Private;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace Application.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public Task SendAccountPendingAsync(string toEmail, string userName) =>
        SendAsync(
            toEmail,
            "Tu cuenta en ALPU está pendiente de aprobación",
            $"""
            <p>Hola {userName},</p>
            <p>Tu cuenta fue creada exitosamente y está pendiente de aprobación por un administrador.</p>
            <p>Te notificaremos cuando sea habilitada.</p>
            <p>— ALPU</p>
            """
        );

    public Task SendAccountApprovedAsync(string toEmail, string userName) =>
        SendAsync(
            toEmail,
            "Tu cuenta en ALPU fue aprobada",
            $"""
            <p>Hola {userName},</p>
            <p>Tu cuenta fue aprobada. Ya podés ingresar al sistema.</p>
            <p>— ALPU</p>
            """
        );

    private async Task SendAsync(string toEmail, string subject, string htmlBody)
    {
        var section = _config.GetSection("Email");

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(
            section["SenderName"], section["SenderEmail"]));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = htmlBody };

        using var client = new SmtpClient();
        await client.ConnectAsync(
            section["SmtpHost"],
            int.Parse(section["SmtpPort"]!),
            SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(
            section["SenderEmail"],
            section["AppPassword"]);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
