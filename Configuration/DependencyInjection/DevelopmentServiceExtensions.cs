using NewsflowApi.Configuration.Settings;

namespace NewsflowApi.Configuration.DependencyInjection
{
    public static class DevelopmentServiceExtensions
    {
        public static IServiceCollection AddNewsflowDevelopment(this IServiceCollection services, IConfiguration configuration)
        {
            var developmentAdminSection = configuration.GetRequiredSection(DevelopmentAdminSettings.SectionName);

            services.AddOptions<DevelopmentAdminSettings>().Bind(developmentAdminSection).ValidateDataAnnotations().ValidateOnStart();

            return services;
        }
    }
}
