using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using NewsflowApi.Application.Authentication;
using NewsflowApi.Application.Staffs;
using NewsflowApi.Domain.Entities.Identity.Users;
using NewsflowApi.Domain.Entities.Staffs;
using NewsflowApi.Domain.Enums;
using NewsflowApi.Domain.Enums.Identity.Users;
using NewsflowApi.Infrastructure.Persistence;
using NewsflowApi.IntegrationTests.Infrastructure;
using NewsflowApi.IntegrationTests.Infrastructure.Email;
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
        public async Task CreateUser_WhenUserAlreadyExists_ShouldReturnConflict()
        {
            using var scope = _factory.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<NewsflowDbContext>();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            var email = $"dup-user-{Guid.NewGuid()}@newsflow.test";

            var staff = new Staff
            {
                Id = Guid.NewGuid(),
                FirstName = "Integration",
                LastName = "Test",
                Email = email,
                ContactPhone = "85912345678"
            };

            context.Add(staff);

            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var request = new CreateUserForStaffRequest
            {
                Email = email,
                StaffId = staff.Id
            };

            var client = _factory.CreateClient();

            var firstResponse = await client.PostAsJsonAsync(
                "/api/auth/create-user",
                request,
                TestContext.Current.CancellationToken
                );

            Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

            var secondResponse = await client.PostAsJsonAsync(
                "/api/auth/create-user",
                request,
                TestContext.Current.CancellationToken
                );

            Assert.Equal(HttpStatusCode.Conflict, secondResponse.StatusCode);
        }

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
                "/api/auth/create-user",
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

        [Fact]
        public async Task MePassword_SuccessfulPasswordChange_ShouldReturnNoResponse()
        {
            using var scope = _factory.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<NewsflowDbContext>();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            var email = $"change-password-{Guid.NewGuid()}@newsflow.test";
            var password = "Test@123";

            var staff = new Staff
            {
                Id = Guid.NewGuid(),
                FirstName = "Integration",
                LastName = "Test",
                Email = email,
                ContactPhone = "8512345678"
            };

            context.Add(staff);

            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var user = new User
            {
                Id = Guid.NewGuid(),
                StaffId = staff.Id,
                Email = staff.Email,
                UserName = staff.Email,
                EmailConfirmed = true,
                Status = UserStatus.Active
            };

            var userCreationResult = await userManager.CreateAsync(user, password);

            Assert.True(userCreationResult.Succeeded);

            var loginRequest = new LoginRequest
            {
                Email = email,
                Password = password
            };

            var client = _factory.CreateClient();

            var loginResponse = await client.PostAsJsonAsync<LoginRequest>(
                "/api/auth/login",
                loginRequest,
                TestContext.Current.CancellationToken
                );

            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            var newPassword = "NewPass@123";

            var changePasswordRequest = new ChangePasswordRequest
            {
                CurrentPassword = password,
                NewPassword = newPassword
            };

            var changePasswordResponse = await client.PostAsJsonAsync<ChangePasswordRequest>(
                "/api/auth/me/password",
                changePasswordRequest,
                TestContext.Current.CancellationToken
                );

            Assert.Equal(HttpStatusCode.NoContent, changePasswordResponse.StatusCode);

            var loginWithNewPassword = new LoginRequest
            {
                Email = email,
                Password = newPassword
            };

            var newPasswordLoginResponse = await client.PostAsJsonAsync<LoginRequest>(
                "/api/auth/login",
                loginWithNewPassword,
                TestContext.Current.CancellationToken
                );

            Assert.Equal(HttpStatusCode.OK, newPasswordLoginResponse.StatusCode);
        }

        [Fact]
        public async Task ForgotPassword_SuccessfulPasswordResetRequest_ShouldReturnOk()
        {
            using var scope = _factory.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<NewsflowDbContext>();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            var mockEmailService = scope.ServiceProvider.GetRequiredService<MockEmailService>();

            var email = $"forgot-password-{Guid.NewGuid()}@newsflow.test";
            var password = "Test@123";

            var staff = new Staff
            {
                Id = Guid.NewGuid(),
                FirstName = "Integration",
                LastName = "Test",
                Email = email,
                ContactPhone = "85912345678"
            };

            context.Add(staff);

            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var user = new User
            {
                Id = Guid.NewGuid(),
                StaffId = staff.Id,
                Email = staff.Email,
                UserName = staff.Email,
                EmailConfirmed = true,
                Status = UserStatus.Active
            };

            var userCreationResult = await userManager.CreateAsync(user, password);

            Assert.True(userCreationResult.Succeeded);

            var client = _factory.CreateClient();

            var request = new RequestPasswordResetRequest
            {
                Email = email,
            };

            var passwordResetResponse = await client.PostAsJsonAsync(
                "/api/auth/forgot-password",
                request,
                TestContext.Current.CancellationToken
                );

            var responseContent = await passwordResetResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.OK, passwordResetResponse.StatusCode);

            Assert.Single(mockEmailService.SentEmails);

            Assert.True(
                passwordResetResponse.IsSuccessStatusCode,
                $"Status: {passwordResetResponse.StatusCode}\n Content: {responseContent}"
                );
        }

        [Fact]
        public async Task ResetPassword_WhenTokenIsValid_ShouldReturnNoContent()
        {
            using var scope = _factory.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<NewsflowDbContext>();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            var email = $"reset-password-{Guid.NewGuid()}@newsflow.test";
            var password = "Test@123";

            var staff = new Staff
            {
                Id = Guid.NewGuid(),
                FirstName = "Integration",
                LastName = "Test",
                Email = email,
                ContactPhone = "85912345678"
            };

            context.Add(staff);

            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var user = new User
            {
                Id = Guid.NewGuid(),
                StaffId = staff.Id,
                Email = staff.Email,
                UserName = staff.Email,
                EmailConfirmed = true,
                Status = UserStatus.Active
            };

            var userCreationResult = await userManager.CreateAsync(user, password);

            Assert.True(userCreationResult.Succeeded);

            var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);

            var encodedToken = PasswordResetTokenCodec.Encode(resetToken);

            var newPassword = "NewPass@123";

            var resetRequest = new ResetPasswordRequest
            {
                Email = email,
                NewPassword = newPassword,
                Token = encodedToken
            };

            var client = _factory.CreateClient();

            var resetResponse = await client.PostAsJsonAsync(
                "/api/auth/reset-password",
                resetRequest,
                TestContext.Current.CancellationToken
                );

            Assert.Equal(HttpStatusCode.NoContent, resetResponse.StatusCode);

            var loginRequest = new LoginRequest
            {
                Email = email,
                Password = newPassword,
            };

            var loginResponse = await client.PostAsJsonAsync(
                "/api/auth/login",
                loginRequest,
                TestContext.Current.CancellationToken
                );

            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        }

        [Fact]
        public async Task ResetPassword_WhenTokenIsNotValid_ShouldReturnBadRequest()
        {
            using var scope = _factory.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<NewsflowDbContext>();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            var email = $"reset-password-{Guid.NewGuid()}@newsflow.test";
            var password = "Test@123";

            var staff = new Staff
            {
                Id = Guid.NewGuid(),
                FirstName = "Integration",
                LastName = "Test",
                Email = email,
                ContactPhone = "85912345678"
            };

            context.Add(staff);

            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var user = new User
            {
                Id = Guid.NewGuid(),
                StaffId = staff.Id,
                Email = staff.Email,
                UserName = staff.Email,
                EmailConfirmed = true,
                Status = UserStatus.Active
            };

            var userCreationResult = await userManager.CreateAsync(user, password);

            Assert.True(userCreationResult.Succeeded);

            var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);

            var invalidToken = $"{resetToken}-invalid_token";

            var encodedToken = PasswordResetTokenCodec.Encode(invalidToken);

            var newPassword = "NewPass@123";

            var request = new ResetPasswordRequest
            {
                Email = email,
                NewPassword = newPassword,
                Token = encodedToken
            };

            var client = _factory.CreateClient();

            var resetResponse = await client.PostAsJsonAsync(
                "/api/auth/reset-password",
                request,
                TestContext.Current.CancellationToken
                );

            Assert.Equal(HttpStatusCode.BadRequest, resetResponse.StatusCode);
        }
    }
}