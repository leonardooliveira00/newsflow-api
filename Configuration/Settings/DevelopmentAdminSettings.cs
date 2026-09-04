using System.ComponentModel.DataAnnotations;

namespace NewsflowApi.Configuration.Settings
{
    public class DevelopmentAdminSettings
    {
        public const string SectionName = "DevelopmentAdmin";

        [Required]
        public required string Password { get; init; }
    }
}
