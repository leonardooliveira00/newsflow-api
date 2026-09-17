using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using NewsflowApi.Domain.Entities.Identity.Users;
using System.Security.Claims;

namespace NewsflowApi.Application.Authorization
{
    public sealed class NewsflowUserClaimsPrincipalFactory(UserManager<User> userManager, IOptions<IdentityOptions> optionsAccessor, AuthorizationService authorizationService) : UserClaimsPrincipalFactory<User>(userManager, optionsAccessor)
    {
        private readonly AuthorizationService _authorizationService = authorizationService;

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User user)
        {
            var identity = await base.GenerateClaimsAsync(user);

            var authorizationResult = await _authorizationService.GetUserAuthorizationAsync(user.Id);

            if (!authorizationResult.Succeeded || authorizationResult.Data is null)
                throw new InvalidOperationException("Failed to retrieve user roles and permissions.");

            identity.AddClaims(
                authorizationResult.Data.Roles.Select(role =>
                new Claim(ClaimTypes.Role, role)
                ));

            identity.AddClaims(
                authorizationResult.Data.Permissions.Select(permission =>
                new Claim(AuthorizationClaimTypes.Permission, permission)
                ));

            return identity;
        }
    }
}
