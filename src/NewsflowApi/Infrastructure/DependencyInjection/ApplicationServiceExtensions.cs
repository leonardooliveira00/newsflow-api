using FluentValidation;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using NewsflowApi.Application.Authentication;
using NewsflowApi.Application.Authorization;
using NewsflowApi.Application.Staffs;
using NewsflowApi.Infrastructure.Email;
using NewsflowApi.Infrastructure.Settings;
using NewsflowApi.Presentation.Dtos.Requests.Staffs;
using NewsflowApi.Presentation.Validations.Staffs;

namespace NewsflowApi.Infrastructure.DependencyInjection
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddNewsflowApplication(
            this IServiceCollection services,
            IConfiguration configuration
            )
        {
            ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Continue;
            ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
            services.AddValidatorsFromAssemblyContaining<RegisterStaffRequestValidator>();
            services.AddFluentValidationAutoValidation();

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
