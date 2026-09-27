using NewsflowApi.Infrastructure.Email;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace NewsflowApi.IntegrationTests.Infrastructure.Email
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
