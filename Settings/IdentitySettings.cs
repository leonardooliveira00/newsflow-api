using System.ComponentModel.DataAnnotations;

namespace NewsflowApi.Settings
{
    public class IdentitySettings
    {
        public const string SectioName = "Identity";

        public bool RequireUniqueEmail { get; set; }

        public bool RequireConfirmedEmail { get; set; }

        [Range(1, 20)]
        public int MaxFailedAccessAttempts { get; set; }

        [Range(1, 1440)]
        public int LockoutMinutes { get; set; }

        public bool AllowedForNewUsers { get; set; }
    }
}
