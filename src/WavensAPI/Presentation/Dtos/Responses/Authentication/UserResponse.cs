using WavensApi.Domain.Enums.Identity.Users;

namespace WavensApi.Presentation.Dtos.Responses.Authentication
{
    public sealed record UserResponse
    {
        public Guid StaffId { get; init; }
        public Guid Id { get; init; }
        public string? Email { get; init; }
        public UserStatus Status { get; init; }
    }
}
