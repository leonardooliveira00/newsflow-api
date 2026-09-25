using Microsoft.AspNetCore.Identity;

namespace NewsflowApi.Infrastructure.Identity
{
    public class EmailNormalizerExtension : ILookupNormalizer
    {
        public string? NormalizeName(string? name)
        {
            return NormalizeEmail(name);
        }

        public string? NormalizeEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return null;
            }

            return email.Trim().ToLowerInvariant();
        }
    }
}
