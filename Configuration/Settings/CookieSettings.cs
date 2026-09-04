using System.ComponentModel.DataAnnotations;

namespace NewsflowApi.Configuration.Settings
{
    public class CookieSettings
    {
        public const string SectionName = "Authentication:Cookie";

        [Required]
        public required string Name { get; set; }

        public bool HttpOnly { get; set; }

        public CookieSecurePolicy SecurePolicy { get; set; }

        public SameSiteMode SameSite { get; set; }

        [Range(1, 168)]
        public int ExpireHours { get; set; }

        public bool SlidingExpiration { get; set; }
    }
}
