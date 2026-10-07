using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using WavensApi.Domain.Entities.Identity.Users;
using WavensApi.Domain.Entities.Staffs;
using WavensApi.Domain.Enums.Identity.Users;
using WavensApi.Infrastructure.Persistence;
using WavensApi.IntegrationTests.Infrastructure;
using WavensApi.Presentation.Dtos.Requests.Authentication;
using WavensApi.Presentation.Dtos.Responses.Authentication;
using System.Net;
using System.Net.Http.Json;

namespace WavensApi.IntegrationTests.Users
{
    public class UsersTests(WavensWebApplicationFactory factory) : IClassFixture<WavensWebApplicationFactory>
    {
        private readonly WavensWebApplicationFactory _factory = factory;

        [Fact]
        public async Task CreateUser_WhenUserAlreadyExists_ShouldReturnConflict()
        {
            using var scope = _factory.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<WavensDbContext>();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            var email = $"dup-user-{Guid.NewGuid()}@wavens.test";

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

            var context = scope.ServiceProvider.GetRequiredService<WavensDbContext>();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            var email = $"create-user-{Guid.NewGuid()}@wavens.test";

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
