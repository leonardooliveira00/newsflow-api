using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NewsflowApi.Data;
using NewsflowApi.Domain.Identity.Users;

namespace NewsflowApi.Extensions;

public static class IdentityServiceExtensions
{
    public static IServiceCollection AddNewsflowIdentity(
        this IServiceCollection services)
    {
        services
            .AddIdentityCore<User>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;

                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan =
                    TimeSpan.FromMinutes(15);
                options.Lockout.AllowedForNewUsers = true;
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
            options.Cookie.Name = "newsflow_auth";
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy =
                CookieSecurePolicy.SameAsRequest;
            options.Cookie.SameSite = SameSiteMode.Lax;

            options.ExpireTimeSpan = TimeSpan.FromHours(8);
            options.SlidingExpiration = true;
        });

        return services;
    }
}