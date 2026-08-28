using NewsflowApi.Domain.Users;
using NewsflowApi.Domain.Roles;

namespace NewsflowApi.Domain.UserRoles
{
    public class UserRole
    {
        public Guid UserId { get; set; }

        public Guid RoleId { get; set; }

        public DateTime AssignedAt { get; set; } = DateTime.Now;

        public User User { get; set; } = null!;

        public Role Role { get; set; } = null!;
    }
}
