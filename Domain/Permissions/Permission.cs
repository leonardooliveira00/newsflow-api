using NewsflowApi.Domain.Common;

namespace NewsflowApi.Domain.Permissions
{
    public class Permission : AuditableEntity
    {
        public Guid Id { get; set; }

        public required string Name { get; set; }

        public string? Description { get; set; }
    }
}
