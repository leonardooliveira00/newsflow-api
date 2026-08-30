using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NewsflowApi.Data;
using NewsflowApi.Domain.Identity.Users;
using NewsflowApi.Settings;

namespace NewsflowApi.Extensions;

public static class IdentityServiceExtensions
{
    public static IServiceCollection AddNewsflowIdentity(
        this IServiceCollection services,
        IConfiguration configuration
        )
    {
        var identitySection = configuration.GetRequiredSection(IdentitySettings.SectioName);
        var cookieSection = configuration.GetRequiredSection(CookieSettings.SectionName);

        services.AddOptions<IdentitySettings>().Bind(identitySection).ValidateDataAnnotations().ValidateOnStart();

        services.AddOptions<CookieSettings>().Bind(cookieSection).ValidateDataAnnotations().ValidateOnStart();

        var identitySettings = identitySection.Get<IdentitySettings>()
            ?? throw new InvalidOperationException(
                "Identity Configuration is invalid."
                );

        var cookieSettings = cookieSection.Get<CookieSettings>()
            ?? throw new InvalidOperationException(
                "Cookie Configuration is invalid."
                );

        services
            .AddIdentityCore<User>(options =>
            {
                options.User.RequireUniqueEmail = identitySettings.RequireUniqueEmail;
                options.SignIn.RequireConfirmedEmail = identitySettings.RequireConfirmedEmail;

                options.Lockout.MaxFailedAccessAttempts = identitySettings.MaxFailedAccessAttempts;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(identitySettings.LockoutMinutes);
                options.Lockout.AllowedForNewUsers = identitySettings.AllowedForNewUsers;
            })
            .AddEntityFrameworkStores<NewsflowDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                    IdentityConstants.ApplicationScheme;

                options.DefaultChallengeScheme =
                    IdentityConstants.ApplicationScheme;

                options.DefaultSignInScheme =
                    IdentityConstants.ApplicationScheme;
            })
            .AddIdentityCookies();

        services.AddAuthorization();

        services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.Name = cookieSettings.Name;
            options.Cookie.HttpOnly = cookieSettings.HttpOnly;
            options.Cookie.SecurePolicy = cookieSettings.SecurePolicy;
            options.Cookie.SameSite = cookieSettings.SameSite;

            options.ExpireTimeSpan = TimeSpan.FromHours(cookieSettings.ExpireHours);
            options.SlidingExpiration = cookieSettings.SlidingExpiration;
        });

        return services;
    }
}