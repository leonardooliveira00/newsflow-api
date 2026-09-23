namespace NewsflowApi.Infrastructure.Email
{
    public interface IEmailService
    {
        Task SendEmailAsync(
            string recipient,
            string subject,
            string content
            );
    }
}
