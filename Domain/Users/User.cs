using Microsoft.AspNetCore.Identity;
using NewsflowApi.Domain.Common;
using NewsflowApi.Domain.Staffs;

namespace NewsflowApi.Domain.Users
{
    public class User : IdentityUser<Guid>, IAuditableEntity
    {
        public Guid StaffId { get; set; }

        public UserStatus Status { get; set; } = UserStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public Staff Staff { get; set; } = null!;
    }
}
