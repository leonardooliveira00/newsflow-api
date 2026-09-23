using System.ComponentModel.DataAnnotations;

namespace NewsflowApi.Infrastructure.Settings
{
    public class DevelopmentAdminSettings
    {
        public const string SectionName = "DevelopmentAdmin";

        [Required]
        public required string Password { get; init; }
    }
}
