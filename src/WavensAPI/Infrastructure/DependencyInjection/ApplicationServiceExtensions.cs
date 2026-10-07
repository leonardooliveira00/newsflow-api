using FluentValidation;
using WavensApi.Application.Authentication;
using WavensApi.Application.Authorization;
using WavensApi.Application.Invitation;
using WavensApi.Application.Staffs;
using WavensApi.Application.Users;
using WavensApi.Infrastructure.Email;
using WavensApi.Infrastructure.Settings;
using WavensApi.Presentation.Validations.Staffs;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

namespace WavensApi.Infrastructure.DependencyInjection
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddWavensApplication(
            this IServiceCollection services,
            IConfiguration configuration
            )
        {
            ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Continue;
            ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
            services.AddValidatorsFromAssemblyContaining<RegisterStaffRequestValidator>();
            services.AddFluentValidationAutoValidation();

            services.AddScoped<StaffService>();

            services.AddScoped<UserService>();

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
