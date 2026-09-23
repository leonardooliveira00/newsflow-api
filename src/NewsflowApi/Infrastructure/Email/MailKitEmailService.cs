using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using NewsflowApi.Infrastructure.Settings;

namespace NewsflowApi.Infrastructure.Email
{
    public sealed class MailKitEmailService(IOptions<EmailSettings> options) : IEmailService
    {
        private readonly EmailSettings _settings = options.Value;

        public async Task SendEmailAsync(string recipient, string subject, string content)
        {
            var message = new MimeMessage();

            message.To.Add(MailboxAddress.Parse(recipient));

            message.From.Add(
                new MailboxAddress(
                    _settings.SenderName,
                    _settings.SenderEmail
                    )
                );

            message.Subject = subject;

            message.Body = new TextPart("plain")
            {
                Text = content
            };

            using var smtpClient = new MailKit.Net.Smtp.SmtpClient();

            var socketOptions = _settings.UseTls ? SecureSocketOptions.StartTls : SecureSocketOptions.None;

            await smtpClient.ConnectAsync(
                _settings.Host,
                _settings.Port,
                socketOptions
                );

            if (_settings.UseAuthentication)
            {
                await smtpClient.AuthenticateAsync(
                    _settings.Username,
                    _settings.Password);
            }

            await smtpClient.SendAsync(message);

            await smtpClient.DisconnectAsync(true);
        }
    }
}