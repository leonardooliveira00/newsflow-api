using NewsflowApi.Domain.Common;

namespace NewsflowApi.Domain.Entities.Authorization
{
    public class Permission : AuditableEntity
    {
        public Guid Id { get; set; }

        public required string Name { get; set; }

        public string? Description { get; set; }

        public ICollection<RolePermission> RolePermissions { get; set; } = [];
    }
}
