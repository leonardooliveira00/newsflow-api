using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NewsflowApi.Domain.Entities.Identity.Users;
using NewsflowApi.Domain.Entities.Staffs;
using NewsflowApi.Domain.Enums.Identity.Users;
using NewsflowApi.Infrastructure.Persistence;
using NewsflowApi.IntegrationTests.Infrastructure;
using NewsflowApi.Presentation.Dtos.Requests.Authentication;
using NewsflowApi.Presentation.Dtos.Responses.Authentication;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace NewsflowApi.IntegrationTests.Users
{
    public class UsersTests(NewsflowWebApplicationFactory factory) : IClassFixture<NewsflowWebApplicationFactory>
    {
        private readonly NewsflowWebApplicationFactory _factory = factory;

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
                "/api/users/create-user",
                request,
                TestContext.Current.CancellationToken
                );

            if (firstResponse.StatusCode != HttpStatusCode.Created)
            {
                var body = await firstResponse.Content.ReadAsStringAsync(
                    TestContext.Current.CancellationToken
                );

                throw new Exception(
                    $"Status: {firstResponse.StatusCode}\nBody: {body}"
                );
            }

            Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

            var secondResponse = await client.PostAsJsonAsync(
                "/api/users/create-user",
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
                "/api/users/create-user",
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
    }
}
