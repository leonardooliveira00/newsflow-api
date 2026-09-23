namespace NewsflowApi.Application.Contracts.Requests.Authentication
{
    public sealed record RequestPasswordResetRequest
    {
        public required string Email { get; init; }
    }
}
