using WavensApi.Infrastructure.Email;
using System.Collections.Concurrent;

namespace WavensApi.IntegrationTests.Infrastructure.Email
{
    public class MockEmailService : IEmailService
    {
        public ConcurrentBag<SentEmailDto> SentEmails { get; } = [];

        public Task SendEmailAsync(
            string recipient,
            string subject,
            string content)
        {
            SentEmails.Add(new SentEmailDto(recipient, subject, content));

            return Task.CompletedTask;
        }
    }
}
