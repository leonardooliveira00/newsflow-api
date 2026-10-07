namespace WavensApi.IntegrationTests.Infrastructure.Email
{
    public record SentEmailDto(
        string Recipient,
        string Subject,
        string Content
        );
}
