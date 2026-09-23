using NewsflowApi.Application.Authentication;
using NewsflowApi.Application.Authorization;
using NewsflowApi.Application.Staffs;
using NewsflowApi.Infrastructure.Email;
using NewsflowApi.Infrastructure.Settings;

namespace NewsflowApi.Infrastructure.DependencyInjection
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddNewsflowApplication(
            this IServiceCollection services,
            IConfiguration configuration
            )
        {
            services.AddScoped<StaffService>();

            services.AddScoped<AuthService>();

            services.AddScoped<InvitationService>();

            services.AddScoped<UserRoleManagementService>();

            services.AddScoped<AuthorizationService>();

            services.AddScoped<IEmailService, MailKitEmailService>();

            services.AddOptions<EmailSettings>()
                .Bind(configuration.GetSection(EmailSettings.SectionName)).ValidateOnStart();

            services.AddOptions<FrontendSettings>()
                .Bind(configuration.GetSection(FrontendSettings.SectionName)).ValidateOnStart();

            return services;
        }
    }
}
