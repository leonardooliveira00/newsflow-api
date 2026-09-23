using NewsflowApi.Infrastructure.Settings;

namespace NewsflowApi.Infrastructure.DependencyInjection
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
