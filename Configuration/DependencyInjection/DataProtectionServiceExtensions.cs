using Microsoft.AspNetCore.DataProtection;

namespace NewsflowApi.Configuration.DependencyInjection
{
    public static class DataProtectionServiceExtensions
    {
        public static IServiceCollection AddNewsflowDataProtection(this IServiceCollection services)
        {
            services.
                AddDataProtection()
                .PersistKeysToFileSystem(
                new DirectoryInfo("/app/data-protection-keys")
                ).SetApplicationName("Newsflow");

            return services;
        }
    }
}
