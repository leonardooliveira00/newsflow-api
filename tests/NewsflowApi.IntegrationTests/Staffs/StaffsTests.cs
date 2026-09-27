using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NewsflowApi.Application.Authorization;
using NewsflowApi.Presentation.Dtos.Requests.Authentication;
using NewsflowApi.Presentation.Dtos.Requests.Staffs;
using NewsflowApi.Application.Staffs;
using NewsflowApi.Domain.Entities.Identity.Users;
using NewsflowApi.Domain.Entities.Staffs;
using NewsflowApi.Domain.Enums.Identity.Users;
using NewsflowApi.Infrastructure.Persistence;
using NewsflowApi.IntegrationTests.Infrastructure;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace NewsflowApi.IntegrationTests.Staffs
{
    public class StaffsTests(NewsflowWebApplicationFactory factory) : IClassFixture<NewsflowWebApplicationFactory>
    {
        private readonly NewsflowWebApplicationFactory _factory = factory;


        [Fact]
        public async Task Staffs_WhenRegisteringDuplicateStaff_ShouldReturnConflict()
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
                ContactPhone = "85912345678",
                Bio = "Duplicate Staff Test."
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
                Status = UserStatus.Active
            };

            var createUserResult = await userManager.CreateAsync(user, password);

            Assert.True(createUserResult.Succeeded);

            var adminRole = await context.Roles.FirstAsync(role => role.Name == "ADMIN", TestContext.Current.CancellationToken);

            var assignRole = await userRoleManagementService.AssignRoleAsync(user.Id, adminRole.Id);

            Assert.True(assignRole.Succeeded);

            var loginRequest = new LoginRequest
            {
                Email = email,
                Password = password,
            };

            var client = _factory.CreateClient();

            var loginResponse = await client.PostAsJsonAsync("/api/auth/login", loginRequest, TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            var registerStaffRequest = new RegisterStaffRequest
            {
                FirstName = "Staff",
                LastName = "Register",
                Email = $"staff-{Guid.NewGuid()}@newsflow.test",
                ContactPhone = "8512345678",
                Bio = ""
            };

            var firstResponse = await client.PostAsJsonAsync("/api/staffs", registerStaffRequest, TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

            var secondResponse = await client.PostAsJsonAsync("/api/staffs", registerStaffRequest, TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.Conflict, secondResponse.StatusCode);
        }
    }
}
