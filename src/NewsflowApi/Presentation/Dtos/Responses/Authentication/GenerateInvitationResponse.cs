namespace NewsflowApi.Presentation.Dtos.Responses.Authentication
{
    public sealed record GenerateInvitationResponse
    {
        public required string Token { get; init; }
    }
}
