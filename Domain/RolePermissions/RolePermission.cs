using NewsflowApi.Domain.Permissions;
using NewsflowApi.Domain.Roles;

namespace NewsflowApi.Domain.RolePermissions
{
    public class RolePermission
    {
        public Guid RoleId { get; set; }

        public Guid PermissionId { get; set; }

        public Role Role { get; set; } = null!;

        public Permission Permission { get; set; } = null!;
    }
}
