namespace NewsflowApi.Application.Contracts.Requests.Authentication
{
    public sealed record ResetPasswordRequest
    {
        public required string Email { get; init; }
        public required string Token { get; init; }
        public required string NewPassword { get; init; }
    }
}
