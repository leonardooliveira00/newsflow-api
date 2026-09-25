using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NewsflowApi.Application.Contracts.Requests.Authentication;
using NewsflowApi.Application.Contracts.Requests.Staffs;
using NewsflowApi.Application.Staffs;
using NewsflowApi.Domain.Enums;
using NewsflowApi.Domain.Entities.Identity.Users;
using NewsflowApi.Domain.Entities.Staffs;
using NewsflowApi.IntegrationTests.Infrastructure;
using System.Net;
using System.Net.Http.Json;
using NewsflowApi.Domain.Enums.Identity.Users;
using NewsflowApi.Infrastructure.Persistence;
using NewsflowApi.Presentation.Dtos.Responses.Authentication;

namespace NewsflowApi.IntegrationTests.Authentication
{
    public class AuthenticationTests(NewsflowWebApplicationFactory factory) : IClassFixture<NewsflowWebApplicationFactory>
    {
        private readonly NewsflowWebApplicationFactory _factory = factory;

        [Fact]
        public async Task Me_WhenUserIsNotAuthenticated_ShouldReturnUnauthorized()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/auth/me");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Login_WhenCredentialsAreInvalid_ShouldReturnUnauthorized()
        {
            var client = _factory.CreateClient();

            var request = new LoginRequest
            {
                Email = "invalid@email.com",
                Password = "invalid_password",
            };

            var response = await client.PostAsJsonAsync("/api/auth/login", request);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ShouldReturnOk()
        {
            using var scope = _factory.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<NewsflowDbContext>();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            var email = $"login-{Guid.NewGuid()}@newsflow.test";
            var password = "Test@123";

            var staff = new Staff
            {
                Id = Guid.NewGuid(),
                FirstName = "Integration",
                LastName = "Test",
                Bio = "Bio de testes de integração.",
                ContactPhone = "85912345678",
                Email = email,
            };

            context.Staffs.Add(staff);

            await context.SaveChangesAsync();

            var user = new User
            {
                Id = Guid.NewGuid(),
                StaffId = staff.Id,
                Email = staff.Email,
                UserName = staff.Email,
                EmailConfirmed = true,
                Status = UserStatus.Active,
            };

            var createUserResult = await userManager.CreateAsync(user, password);

            Assert.True(createUserResult.Succeeded);

            var request = new LoginRequest
            {
                Email = email,
                Password = password
            };

            var client = _factory.CreateClient();

            var response = await client.PostAsJsonAsync("/api/auth/login", request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ShouldAuthenticate()
        {
            using var scope = _factory.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<NewsflowDbContext>();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            var email = $"login-{Guid.NewGuid()}@newsflow.test";
            var password = "Test@123";

            var staff = new Staff
            {
                Id = Guid.NewGuid(),
                FirstName = "Integration",
                LastName = "Test",
                Bio = "Bio de testes de integração.",
                ContactPhone = "85912345678",
                Email = email,
            };

            context.Staffs.Add(staff);

            await context.SaveChangesAsync();

            var user = new User
            {
                Id = Guid.NewGuid(),
                StaffId = staff.Id,
                Email = staff.Email,
                UserName = staff.Email,
                EmailConfirmed = true,
                Status = UserStatus.Active,
            };

            var createUserResult = await userManager.CreateAsync(user, password);

            Assert.True(createUserResult.Succeeded);

            var request = new LoginRequest
            {
                Email = email,
                Password = password
            };

            var client = _factory.CreateClient();

            var loginResponse = await client.PostAsJsonAsync("/api/auth/login", request);

            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            var meResponse = await client.GetAsync("/api/auth/me");

            Assert.Equal(HttpStatusCode.OK, meResponse.StatusCode);
        }

        [Fact]
        public async Task Login_WhenUserAcceptsInvitation_ShouldReturnOk()
        {
            using var scope = _factory.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<NewsflowDbContext>();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            var email = $"login-{Guid.NewGuid()}@newsflow.test";
            var password = "Test@123";

            var staff = new Staff
            {
                Id = Guid.NewGuid(),
                FirstName = "Integration",
                LastName = "Test",
                Bio = "Bio de testes de integração.",
                ContactPhone = "85912345678",
                Email = email,
            };

            context.Staffs.Add(staff);

            await context.SaveChangesAsync();

            var user = new User
            {
                Id = Guid.NewGuid(),
                StaffId = staff.Id,
                Email = staff.Email,
                UserName = staff.Email,
                EmailConfirmed = false,
                Status = UserStatus.Pending
            };

            var createUserResult = await userManager.CreateAsync(user);

            Assert.True(createUserResult.Succeeded);

            var client = _factory.CreateClient(); 

            var generateInvitationResponse = await client.PostAsync($"/api/invitations/{user.Id}", null);

            Assert.Equal(HttpStatusCode.OK, generateInvitationResponse.StatusCode);

            var invitationResponse = await generateInvitationResponse.Content.ReadFromJsonAsync<GenerateInvitationResponse>();

            Assert.NotNull(invitationResponse);

            var token = invitationResponse.Token;

            var acceptInvitationRequest = new AcceptInvitationRequest
            {
                UserId = user.Id,
                Token = token,
                Password = password,
            };

            var acceptInvitationResponse = await client.PostAsJsonAsync("/api/invitations/accept", acceptInvitationRequest);

            Assert.Equal(HttpStatusCode.OK , acceptInvitationResponse.StatusCode);

            var loginRequest = new LoginRequest
            {
                Email = email,
                Password = password,
            };

            var loginResponse = await client.PostAsJsonAsync("/api/auth/login", loginRequest);

            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        }
    }

}