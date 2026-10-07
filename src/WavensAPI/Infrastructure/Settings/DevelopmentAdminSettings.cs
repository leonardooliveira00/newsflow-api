using System.ComponentModel.DataAnnotations;

namespace WavensApi.Infrastructure.Settings
{
    public class DevelopmentAdminSettings
    {
        public const string SectionName = "DevelopmentAdmin";

        [Required]
        public required string Password { get; init; }
    }
}
