using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NewsflowApi.Application.Authorization;
using NewsflowApi.Application.Contracts.Requests.Staffs;
using NewsflowApi.Domain.Entities.Identity.Users;
using NewsflowApi.Domain.Entities.Staffs;
using NewsflowApi.Domain.Enums.Identity.Users;
using NewsflowApi.Infrastructure.Persistence;
using NewsflowApi.IntegrationTests.Infrastructure;
using System.Net;
using System.Net.Http.Json;

namespace NewsflowApi.IntegrationTests.Authorization
{
    public class AuthorizationTests(NewsflowWebApplicationFactory factory) : IClassFixture<NewsflowWebApplicationFactory>
    {
        private readonly NewsflowWebApplicationFactory _factory = factory;

        [Fact]
        public async Task CreateStaff_WhenUserDoesNotHavePermission_ShouldReturnForbidden()
        {
            using var scope = _factory.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<NewsflowDbContext>();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            var userRoleManagementService = scope.ServiceProvider.GetRequiredService<UserRoleManagementService>();

            var email = $"login-{Guid.NewGuid()}@newsflow.test";
            var password = "Test@123";

            var staff = new Staff
            {
                Id = Guid.NewGuid(),
                FirstName = "Integration",
                LastName = "Test",
                Email = email,
                ContactPhone = "85987654321",
                Bio = "Authorization Test"
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
                Status = UserStatus.Active
            };

            var createResult = await userManager.CreateAsync(user, password);

            Assert.True(createResult.Succeeded);

            var reporterRole = await context.Roles.FirstAsync(role => role.Name == "REPORTER");

            var assignRoleResult = await userRoleManagementService.AssignRoleAsync(user.Id, reporterRole.Id);

            Assert.True(assignRoleResult.Succeeded);

            var loginRequest = new LoginRequest
            {
                Email = email,
                Password = password,
            };

            var client = _factory.CreateClient();

            var loginResponse = await client.PostAsJsonAsync("/api/auth/login", loginRequest);

            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            var registerStaffRequest = new RegisterStaffRequest
            {
                FirstName = "Staff",
                LastName = "Register",
                Email = $"staff-{Guid.NewGuid()}@newsflow.test",
                ContactPhone = "8512345678",
                Bio = ""
            };

            var registerStaffResponse = await client.PostAsJsonAsync("/api/staffs", registerStaffRequest);

            Assert.Equal(HttpStatusCode.Forbidden, registerStaffResponse.StatusCode);
        }

        [Fact]
        public async Task CreateStaff_WhenUserHavePermission_ShouldReturnCreated()
        {
            using var scope = _factory.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<NewsflowDbContext>();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            var userRoleManagementService = scope.ServiceProvider.GetRequiredService<UserRoleManagementService>();

            var email = $"login-{Guid.NewGuid()}@newsflow.test";
            var password = "Test@123";

            var staff = new Staff
            {
                Id = Guid.NewGuid(),
                FirstName = "Integration",
                LastName = "Test",
                Email = email,
                ContactPhone = "8512345678",
                Bio = "Authorization Test",
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
                Status = UserStatus.Active
            };

            var createResult = await userManager.CreateAsync(user, password);

            Assert.True(createResult.Succeeded);

            var adminRole = await context.Roles.FirstAsync(role => role.Name == "ADMIN");

            var assignRoleResult = await userRoleManagementService.AssignRoleAsync(user.Id, adminRole.Id);

            Assert.True(assignRoleResult.Succeeded);

            var loginRequest = new LoginRequest
            {
                Email = email,
                Password = password,
            };

            var client = _factory.CreateClient();

            var loginResponse = await client.PostAsJsonAsync("/api/auth/login", loginRequest);

            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            var registerStaffRequest = new RegisterStaffRequest
            {
                FirstName = "Staff",
                LastName = "Register",
                Email = $"staff-{Guid.NewGuid()}@newsflow.test",
                ContactPhone = "8512345678",
                Bio = ""
            };

            var registerStaffResponse = await client.PostAsJsonAsync("/api/staffs", registerStaffRequest);

            Assert.Equal(HttpStatusCode.Created, registerStaffResponse.StatusCode);
        }

        [Fact]
        public async Task UserRole_WhenRoleIsRemoved_ShouldInvalidateExistingSession()
        {
            using var scope = _factory.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<NewsflowDbContext>();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            var userRoleManagementService = scope.ServiceProvider.GetRequiredService<UserRoleManagementService>();

            var email = $"login-{Guid.NewGuid()}@newsflow.test";
            var password = "Test@123";

            var staff = new Staff
            {
                Id = Guid.NewGuid(),
                FirstName = "Integration",
                LastName = "Test",
                Email = email,
                ContactPhone = "8512345678",
                Bio = "Authorization Test",
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

            var adminRole = await context.Roles.FirstAsync(role => role.Name == "ADMIN");

            var assignAdminRoleResult = await userRoleManagementService.AssignRoleAsync(user.Id, adminRole.Id);

            Assert.True(assignAdminRoleResult.Succeeded);

            var loginRequest = new LoginRequest
            {
                Email = email,
                Password = password,
            };

            var client = _factory.CreateClient();

            var loginResponse = await client.PostAsJsonAsync("/api/auth/login", loginRequest);

            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            var registerStaffRequest = new RegisterStaffRequest
            {
                FirstName = "Staff",
                LastName = "Register",
                Email = $"staff-{Guid.NewGuid()}@newsflow.test",
                ContactPhone = "8512345678",
                Bio = ""
            };

            var registerStaffResponse = await client.PostAsJsonAsync("/api/staffs", registerStaffRequest);

            Assert.Equal(HttpStatusCode.Created, registerStaffResponse.StatusCode);

            var removeRoleResponse = await client.DeleteAsync($"/api/users/{user.Id}/roles/{adminRole.Id}");

            Assert.Equal(HttpStatusCode.NoContent, removeRoleResponse.StatusCode);

            var roleRemovedRegisterStaffRequest = new RegisterStaffRequest
            {
                FirstName = "New",
                LastName = "Staff",
                Email = $"staff-{Guid.NewGuid()}@newsflow.test",
                ContactPhone = "85987654321",
            };

            var roleRemovedRegisterStaffResponse = await client.PostAsJsonAsync("/api/staffs", roleRemovedRegisterStaffRequest);

            Assert.Equal(HttpStatusCode.Unauthorized, roleRemovedRegisterStaffResponse.StatusCode);
        }
    }
}
