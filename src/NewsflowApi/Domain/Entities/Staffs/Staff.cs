using NewsflowApi.Domain.Common;
using NewsflowApi.Domain.Entities.Identity.Users;
using NewsflowApi.Domain.Enums.Staffs;

namespace NewsflowApi.Domain.Entities.Staffs
{
    public class Staff : AuditableEntity
    {
        public Guid Id { get; set; }

        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        public required string Email { get; set; }

        public required string ContactPhone { get; set; }

        public string? Bio { get; set; }

        public StaffStatus Status { get; set; } = StaffStatus.Active;

        public User? User { get; set; }
    }
}
