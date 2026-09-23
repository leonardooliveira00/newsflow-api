namespace NewsflowApi.Infrastructure.Settings
{
    public class EmailSettings
    {
        public const string SectionName = "Email";

        public required string Host { get; set; }
        public int Port { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string SenderEmail { get; set; }
        public required string SenderName { get; set; }
        public bool UseAuthentication { get; set; }
        public bool UseTls { get; set; }
    }
}
