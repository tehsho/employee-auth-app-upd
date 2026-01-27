using EmployeeAuth.Api.Domain.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace EmployeeAuth.Infrastructure.Email;

public class SmtpEmailSender : IEmailSender
{
    private readonly EmailOptions _opt;

    public SmtpEmailSender(IOptions<EmailOptions> opt, ILogger<ConsoleEmailSender> logger)
    {
        _opt = opt.Value;
        _logger = logger;
    }

    private readonly ILogger<ConsoleEmailSender> _logger;

    public async Task SendAsync(string toEmail, string subject, string body)
    {
        _logger.LogInformation("Sending email via SMTP to {To} (Subject: {Subject})", toEmail, subject);

        try
        {
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(_opt.From));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;

            message.Body = new TextPart("plain") { Text = body };

            using var client = new SmtpClient();

            var tls = _opt.Smtp.UseStartTls ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto;

            await client.ConnectAsync(_opt.Smtp.Host, _opt.Smtp.Port, tls);
            await client.AuthenticateAsync(_opt.Smtp.User, _opt.Smtp.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SMTP email send failed to {To}", toEmail);
            throw;
        }
    }
}
