namespace NewsflowApi.Presentation.Dtos.Requests.Authentication
{
    public sealed record RequestPasswordResetRequest
    {
        public required string Email { get; init; }
    }
}
