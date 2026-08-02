using Domain.Interfaces.Private;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace Application.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    private string LogoPath = null; // ruta física en disco (relativa a la raíz de la app)
    private const string LogoCid = "alpu-logo";
    private const string LoginUrl = "https://TU_DOMINIO.com/login";

    // Paleta basada en el logo de ALPU
    private const string ColorPrimary = "#1565C0";
    private const string ColorPrimaryDark = "#0D47A1";
    private const string ColorTextDark = "#1F2937";
    private const string ColorTextMuted = "#6B7280";
    private const string ColorBackground = "#F2F4F7";
    private const string ColorCard = "#FFFFFF";
    private const string ColorBorder = "#E5E7EB";
    private const string ColorInfoBg = "#EAF2FB";
    private const string ColorInfoBorder = "#BBD9F5";


    public EmailService(IConfiguration config)
    {
        _config = config;
        LogoPath = _config["Static:LogoPath"] ?? throw new InvalidOperationException("No se encontró la ruta del logo en la configuración.");
    }
    

    /// <summary>
    /// Genera el layout base (header con logo, tarjeta blanca y footer) que comparten
    /// todos los correos transaccionales de ALPU. El logo se referencia como "cid:alpu-logo",
    /// no como URL, porque va embebido en el propio mensaje.
    /// </summary>
    private static string BuildEmailLayout(string title, string bodyHtml) => $$"""
    <!DOCTYPE html>
    <html lang="es">
    <head>
      <meta charset="utf-8" />
      <meta name="viewport" content="width=device-width, initial-scale=1.0" />
      <meta name="color-scheme" content="light" />
      <title>{{title}}</title>
      <style>
        @media only screen and (max-width: 620px) {
          .alpu-container { width: 100% !important; }
          .alpu-card { border-radius: 0 !important; }
          .alpu-padding { padding: 24px !important; }
        }
      </style>
    </head>
    <body style="margin:0; padding:0; background-color:{{ColorBackground}}; font-family:'Segoe UI', Arial, Helvetica, sans-serif;">
      <div style="display:none; max-height:0; overflow:hidden; opacity:0;">{{title}}</div>
      <table role="presentation" width="100%" cellpadding="0" cellspacing="0" style="background-color:{{ColorBackground}}; padding:32px 16px;">
        <tr>
          <td align="center">
            <table role="presentation" class="alpu-container" width="600" cellpadding="0" cellspacing="0" style="width:600px; max-width:600px;">
 
              <!-- Logo (embebido vía cid, no por URL) -->
              <tr>
                <td align="center" style="padding-bottom:20px;">
                  <img src="cid:{{LogoCid}}" alt="ALPU" width="150" style="display:block; border:0; outline:none; text-decoration:none;" />
                </td>
              </tr>
 
              <!-- Tarjeta -->
              <tr>
                <td class="alpu-card" style="background-color:{{ColorCard}}; border-radius:12px; border:1px solid {{ColorBorder}}; box-shadow:0 2px 10px rgba(16, 42, 87, 0.06);">
                  <table role="presentation" width="100%" cellpadding="0" cellspacing="0">
                    <tr>
                      <td class="alpu-padding" style="padding:40px 40px 32px 40px;">
                        {{bodyHtml}}
                      </td>
                    </tr>
                  </table>
                </td>
              </tr>
 
              <!-- Footer -->
              <tr>
                <td align="center" style="padding-top:24px;">
                  <p style="margin:0; font-size:12px; line-height:18px; color:{{ColorTextMuted}}; font-family:'Segoe UI', Arial, Helvetica, sans-serif;">
                    © {{DateTime.UtcNow.Year}} Asociación de Locutores Profesionales del Uruguay (ALPU)
                  </p>
                  <p style="margin:4px 0 0 0; font-size:12px; line-height:18px; color:{{ColorTextMuted}}; font-family:'Segoe UI', Arial, Helvetica, sans-serif;">
                    Este es un mensaje automático, por favor no respondas a este correo.
                  </p>
                </td>
              </tr>
 
            </table>
          </td>
        </tr>
      </table>
    </body>
    </html>
    """;

    /// <summary>
    /// Igual que tu SendAsync actual, pero además adjunta el logo como recurso vinculado
    /// (Content-Id = "alpu-logo") para que el <img src="cid:alpu-logo"> del HTML lo resuelva
    /// sin depender de ninguna URL pública. Usa exactamente la misma sección de configuración
    /// ("Email") que ya tenés funcionando.
    /// </summary>
    private async Task SendEmailWithLogoAsync(string toEmail, string subject, string htmlBody)
    {
        var section = _config.GetSection("Email");
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(
            section["SenderName"], section["SenderEmail"]));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = subject;

        var builder = new BodyBuilder
        {
            HtmlBody = htmlBody
        };

        // Adjunta el logo como recurso vinculado; su Content-Id coincide con el cid del HTML
        var logo = builder.LinkedResources.Add(LogoPath);
        logo.ContentId = LogoCid;

        message.Body = builder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(
            section["SmtpHost"],
            int.Parse(section["SmtpPort"]!),
            SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(
            section["SmtpUsername"],
            section["AppPassword"]);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }


    public Task SendAccountPendingAsync(string toEmail, string userName)
    {
        var body = $$"""
        <h1 style="margin:0 0 16px 0; font-size:20px; line-height:28px; color:{{ColorTextDark}}; font-family:'Segoe UI', Arial, Helvetica, sans-serif;">
          ¡Hola, {{userName}}!
        </h1>
        <p style="margin:0 0 16px 0; font-size:15px; line-height:24px; color:{{ColorTextDark}}; font-family:'Segoe UI', Arial, Helvetica, sans-serif;">
          Tu cuenta en <strong>ALPU</strong> fue creada correctamente. Antes de que puedas ingresar, uno de nuestros administradores necesita revisar y aprobar tu registro.
        </p>
 
        <table role="presentation" width="100%" cellpadding="0" cellspacing="0" style="margin:8px 0 20px 0;">
          <tr>
            <td style="background-color:{{ColorInfoBg}}; border:1px solid {{ColorInfoBorder}}; border-radius:8px; padding:16px 20px;">
              <p style="margin:0; font-size:14px; line-height:22px; color:{{ColorPrimaryDark}}; font-family:'Segoe UI', Arial, Helvetica, sans-serif;">
                ⏳ No necesitas hacer nada más por ahora. Apenas tu cuenta sea habilitada, te enviaremos un correo para que puedas ingresar al sistema.
              </p>
            </td>
          </tr>
        </table>
 
        <p style="margin:0; font-size:15px; line-height:24px; color:{{ColorTextDark}}; font-family:'Segoe UI', Arial, Helvetica, sans-serif;">
          Gracias por tu paciencia.<br />
          — El equipo de ALPU
        </p>
        """;

        return SendEmailWithLogoAsync(
            toEmail,
            "Tu cuenta en ALPU está pendiente de aprobación",
            BuildEmailLayout("Cuenta pendiente de aprobación", body)
        );
    }

    public Task SendAccountApprovedAsync(string toEmail, string userName)
    {
        var body = $$"""
        <h1 style="margin:0 0 16px 0; font-size:20px; line-height:28px; color:{{ColorTextDark}}; font-family:'Segoe UI', Arial, Helvetica, sans-serif;">
          ¡Buenas noticias, {{userName}}!
        </h1>
        <p style="margin:0 0 24px 0; font-size:15px; line-height:24px; color:{{ColorTextDark}}; font-family:'Segoe UI', Arial, Helvetica, sans-serif;">
          Tu cuenta en <strong>ALPU</strong> fue revisada y ya se encuentra <strong>habilitada</strong>. Ya puedes ingresar al sistema y comenzar a utilizarlo.
        </p>
 
        <table role="presentation" cellpadding="0" cellspacing="0" style="margin:0 0 24px 0;">
          <tr>
            <td style="border-radius:8px; background-color:{{ColorPrimary}};">
              <a href="{{LoginUrl}}" target="_blank" style="display:inline-block; padding:14px 32px; font-size:15px; font-weight:600; color:#FFFFFF; text-decoration:none; font-family:'Segoe UI', Arial, Helvetica, sans-serif; border-radius:8px;">
                Ingresar al sistema
              </a>
            </td>
          </tr>
        </table>
 
        <p style="margin:0; font-size:15px; line-height:24px; color:{{ColorTextDark}}; font-family:'Segoe UI', Arial, Helvetica, sans-serif;">
          Si tienes alguna duda para comenzar, no dudes en contactarnos.<br />
          — El equipo de ALPU
        </p>
        """;

        return SendEmailWithLogoAsync(
            toEmail,
            "Tu cuenta en ALPU fue aprobada",
            BuildEmailLayout("Cuenta aprobada", body)
        );
    }
}
