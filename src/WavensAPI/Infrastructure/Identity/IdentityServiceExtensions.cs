using Microsoft.AspNetCore.Identity;
using WavensApi.Application.Authorization;
using WavensApi.Domain.Constants.Authorization;
using WavensApi.Domain.Entities.Identity.Users;
using WavensApi.Infrastructure.Persistence;
using WavensApi.Infrastructure.Settings;

namespace WavensApi.Infrastructure.Identity;

public static class IdentityServiceExtensions
{
    public static IServiceCollection AddWavensIdentity(
        this IServiceCollection services,
        IConfiguration configuration
        )
    {
        var identitySection = configuration.GetRequiredSection(IdentitySettings.SectionName);
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
            .AddEntityFrameworkStores<WavensDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders()
            .AddClaimsPrincipalFactory<WavensUserClaimsPrincipalFactory>();

        services.AddSingleton<ILookupNormalizer, EmailNormalizerExtension>();

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

        services.AddAuthorizationBuilder()
            .AddPolicy(PermissionConstants.ManageRole, policy => policy.RequireClaim(AuthorizationClaimTypes.Permission, PermissionConstants.ManageRole))
            .AddPolicy(PermissionConstants.CreateStaff, policy => policy.RequireClaim(AuthorizationClaimTypes.Permission, PermissionConstants.CreateStaff))
            .AddPolicy(PermissionConstants.ViewStaff, policy => policy.RequireClaim(AuthorizationClaimTypes.Permission, PermissionConstants.ViewStaff))
            .AddPolicy(PermissionConstants.UpdateStaff, policy => policy.RequireClaim(AuthorizationClaimTypes.Permission, PermissionConstants.UpdateStaff))
            .AddPolicy(PermissionConstants.DeactivateStaff, policy => policy.RequireClaim(AuthorizationClaimTypes.Permission, PermissionConstants.DeactivateStaff));

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