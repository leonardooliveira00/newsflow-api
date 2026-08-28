using NewsflowApi.Domain.Common;
using NewsflowApi.Domain.Users;

namespace NewsflowApi.Domain.Staffs
{
    public class Staff : AuditableEntity
    {
        public Guid Id { get; set; }

        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        public User? User { get; set; }
    }
}
