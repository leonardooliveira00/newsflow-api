using NewsflowApi.Domain.Users;

namespace NewsflowApi.Domain.Common
{
    public class AuditableEntity
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
