namespace NewsflowApi.Infrastructure.Settings
{
    public sealed class FrontendSettings
    {
        public const string SectionName = "Frontend";
        public required string InvitationTokenUrl { get; set; }
        public required string ResetPasswordUrl { get; set; }
    }
}
