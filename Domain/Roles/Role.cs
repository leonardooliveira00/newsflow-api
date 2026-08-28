using NewsflowApi.Domain.Common;

namespace NewsflowApi.Domain.Roles
{
    public class Role : AuditableEntity
    {
        public Guid RoleId { get; set; }

        public required string RoleName { get; set; }

        public string? RoleDescription { get; set; }
    }
}
