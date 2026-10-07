using WavensApi.Infrastructure.Settings;

namespace WavensApi.Infrastructure.DependencyInjection
{
    public static class DevelopmentServiceExtensions
    {
        public static IServiceCollection AddWavensDevelopment(this IServiceCollection services, IConfiguration configuration)
        {
            var developmentAdminSection = configuration.GetRequiredSection(DevelopmentAdminSettings.SectionName);

            services.AddOptions<DevelopmentAdminSettings>().Bind(developmentAdminSection).ValidateDataAnnotations().ValidateOnStart();

            return services;
        }
    }
}
