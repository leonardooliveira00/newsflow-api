using System;
using System.Collections.Generic;
using System.Text;

namespace NewsflowApi.IntegrationTests.Infrastructure.Email
{
    public record SentEmailDto(
        string Recipient,
        string Subject,
        string Content
        );
}
