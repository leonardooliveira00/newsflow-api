using NewsflowApi.Application.Authentication;

namespace NewsflowApi.Extensions
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
