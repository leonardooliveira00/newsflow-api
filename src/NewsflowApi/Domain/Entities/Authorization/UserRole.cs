using NewsflowApi.Domain.Entities.Identity.Users;

namespace NewsflowApi.Domain.Entities.Authorization
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
