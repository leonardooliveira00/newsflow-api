using NewsflowApi.Domain.Common;

namespace NewsflowApi.Domain.Users
{
    public class User : AuditableEntity
    {
        public Guid UserId { get; set; }

        public required string Email { get; set; }

        public string? PasswordHash { get; set; }

        public UserStatus Status { get; set; } = UserStatus.Pending;
    }
}
