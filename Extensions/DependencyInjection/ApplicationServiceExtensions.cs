using NewsflowApi.Application.Authentication;
using NewsflowApi.Application.Authorization;
using NewsflowApi.Application.Staffs;

namespace NewsflowApi.Extensions.DependencyInjection
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddNewsflowApplication(this IServiceCollection services)
        {
            services.AddScoped<StaffService>();

            services.AddScoped<AuthService>();

            services.AddScoped<InvitationService>();

            services.AddScoped<RoleAssignmentService>();

            services.AddScoped<AuthorizationService>();

            return services;
        }
    }
}
