using NewsflowApi.Domain.Common;

namespace NewsflowApi.Domain.Permissions
{
    public class Permission : AuditableEntity
    {
        public Guid PermissionId { get; set; }

        public required string Name { get; set; }

        public string? Description { get; set; }
    }
}
