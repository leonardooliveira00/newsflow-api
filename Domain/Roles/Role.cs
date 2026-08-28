using NewsflowApi.Domain.Common;

namespace NewsflowApi.Domain.Roles
{
    public class Role : AuditableEntity
    {
        public Guid Id { get; set; }

        public required string RoleName { get; set; }

        public string? RoleDescription { get; set; }
    }
}
