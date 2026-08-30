using NewsflowApi.Application.Authentication;

namespace NewsflowApi.Extensions.DependencyInjection
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddNewsflowApplication(this IServiceCollection services)
        {
            services.AddScoped<AuthService>();

            services.AddScoped<InvitationService>();

            return services;
        }
    }
}
