using NewsflowApi.Domain.Common;
using NewsflowApi.Domain.Identity.Users;

namespace NewsflowApi.Domain.Identity.Staffs
{
    public class Staff : AuditableEntity
    {
        public Guid Id { get; set; }

        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        public required string Email { get; set; }

        public required string ContactPhone { get; set; }

        public string? Bio { get; set; }

        public User? User { get; set; }
    }
}
