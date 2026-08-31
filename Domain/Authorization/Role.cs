using NewsflowApi.Domain.Common;

namespace NewsflowApi.Domain.Authorization
{
    public class Role : AuditableEntity
    {
        public Guid Id { get; set; }

        public required string Name { get; set; }

        public string? Description { get; set; }
    }
}
