namespace NewsflowApi.Presentation.Dtos.Requests.Authentication
{
    public sealed record AcceptInvitationRequest
    {
        public Guid UserId { get; init; }
        public required string Token { get; init; }
        public required string Password { get; init; }
    }
}
