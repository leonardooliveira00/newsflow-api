namespace NewsflowApi.Contracts.Authentication
{
    public class AcceptInvitationRequest
    {
        public Guid UserId { get; set; }

        public required string Token { get; set; }

        public required string Password { get; set; }
    }
}
