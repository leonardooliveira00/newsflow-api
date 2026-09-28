using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NewsflowApi.Application.Staffs;
using NewsflowApi.Domain.Entities.Identity.Users;
using NewsflowApi.Domain.Entities.Staffs;
using NewsflowApi.Domain.Enums;
using NewsflowApi.Domain.Enums.Identity.Users;
using NewsflowApi.Infrastructure.Persistence;
using NewsflowApi.IntegrationTests.Infrastructure;
using NewsflowApi.Presentation.Dtos.Requests.Authentication;
using NewsflowApi.Presentation.Dtos.Requests.Staffs;
using NewsflowApi.Presentation.Dtos.Responses.Authentication;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace NewsflowApi.IntegrationTests.Authentication
{
    public class AuthenticationTests(NewsflowWebApplicationFactory factory) : IClassFixture<NewsflowWebApplicationFactory>
    {
        private readonly NewsflowWebApplicationFactory _factory = factory;

        [Fact]
        public async Task CreateUser_CreateUserForStaff_ShouldReturnCreated()
        {
            using var scope = _factory.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<NewsflowDbContext>();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            var email = $"create-user-{Guid.NewGuid()}@newsflow.test";

            var staff = new Staff
            {
                Id = Guid.NewGuid(),
                FirstName = "Create",
                LastName = "User",
                Email = email,
                ContactPhone = "85912345678"
            };

            context.Add(staff);

            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var request = new CreateUserForStaffRequest
            {
                StaffId = staff.Id,
                Email = email
            };

            var client = _factory.CreateClient();

            var createdUserResponse = await client.PostAsJsonAsync(
                $"/api/auth/create-user",
                request,
                TestContext.Current.CancellationToken
                );

            Assert.Equal(HttpStatusCode.Created, createdUserResponse.StatusCode);

            var content = await createdUserResponse.Content.ReadFromJsonAsync<UserResponse>(TestContext.Current.CancellationToken);

            Assert.NotNull(content);

            Assert.NotEqual(Guid.Empty, content.Id);
            Assert.Equal(staff.Id, content.StaffId);
            Assert.Equal(email, content.Email);
            Assert.Equal(UserStatus.Pending, content.Status);
        }

        [Fact]
        public async Task Me_WhenUserIsNotAuthenticated_ShouldReturnUnauthorized()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/auth/me", TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
        {
            var client = _factory.CreateClient();

            var request = new LoginRequest
            {
                Email = "invalid@email.com",
                Password = "invalid_password",
            };

            var response = await client.PostAsJsonAsync("/api/auth/login", request, TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
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

            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

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

            var loginResponse = await client.PostAsJsonAsync("/api/auth/login", request, TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            var meResponse = await client.GetAsync("/api/auth/me", TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.OK, meResponse.StatusCode);
        }

    }
}