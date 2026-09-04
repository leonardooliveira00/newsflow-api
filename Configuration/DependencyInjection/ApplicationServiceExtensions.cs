using NewsflowApi.Application.Authentication;
using NewsflowApi.Application.Authorization;
using NewsflowApi.Application.Staffs;

namespace NewsflowApi.Configuration.DependencyInjection
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddNewsflowApplication(this IServiceCollection services)
        {
            services.AddScoped<StaffService>();

            services.AddScoped<AuthService>();

            services.AddScoped<InvitationService>();

            services.AddScoped<UserRoleManagementService>();

            services.AddScoped<AuthorizationService>();

            return services;
        }
    }
}
