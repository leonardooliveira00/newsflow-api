using Microsoft.AspNetCore.DataProtection;

namespace WavensApi.Infrastructure.DependencyInjection
{
    public static class DataProtectionServiceExtensions
    {
        public static IServiceCollection AddWavensDataProtection(this IServiceCollection services)
        {
            services.
                AddDataProtection()
                .PersistKeysToFileSystem(
                new DirectoryInfo("/app/data-protection-keys")
                ).SetApplicationName("Wavens");

            return services;
        }
    }
}
