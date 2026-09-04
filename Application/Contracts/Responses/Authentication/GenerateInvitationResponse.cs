namespace NewsflowApi.Application.Contracts.Responses.Authentication
{
    public sealed record GenerateInvitationResponse
    {
        public required string Token { get; init; }
    }
}
