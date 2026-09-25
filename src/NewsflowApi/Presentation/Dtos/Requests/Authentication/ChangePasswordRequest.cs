namespace NewsflowApi.Presentation.Dtos.Requests.Authentication
{
    public sealed class ChangePasswordRequest
    {
        public required string CurrentPassword { get; init; }
        public required string NewPassword { get; init; }
    }
}
