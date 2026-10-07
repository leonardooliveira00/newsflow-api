using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WavensApi.Application.Invitation;
using WavensApi.Domain.Entities.Identity.Users;
using WavensApi.Domain.Entities.Staffs;
using WavensApi.Domain.Enums.Identity.Users;
using WavensApi.Infrastructure.Persistence;
using WavensApi.IntegrationTests.Infrastructure;
using WavensApi.IntegrationTests.Infrastructure.Email;
using WavensApi.Presentation.Dtos.Requests.Authentication;
using System.Net;
using System.Net.Http.Json;

namespace WavensApi.IntegrationTests.Invitation
{
    public class InvitationTest(WavensWebApplicationFactory factory) : IClassFixture<WavensWebApplicationFactory>
    {
        private readonly WavensWebApplicationFactory _factory = factory;

        private const string InvitationTokenPurpose = "WavensInvitation";

        [Fact]
        public async Task Invitation_GenerateInvitationWhenUserIsAlreadyActive_ShouldReturnConflict()
        {
            using var scope = _factory.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<WavensDbContext>();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            var email = $"invitation-{Guid.NewGuid()}@wavens.test";

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
                Status = UserStatus.Active,
            };

            var createdUserResult = await userManager.CreateAsync(user);

            Assert.True(createdUserResult.Succeeded);

            var client = _factory.CreateClient();

            var generateInvitationResponse = await client.PostAsync(
                $"/api/invitations/{user.Id}",
                null,
                TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.Conflict, generateInvitationResponse.StatusCode);
        }

        [Fact]
        public async Task Invitation_GenerateInvitationToken_ShouldReturnOk()
        {
            using var scope = _factory.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<WavensDbContext>();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            var mockEmailService = scope.ServiceProvider.GetRequiredService<MockEmailService>();

            var email = $"invitation-{Guid.NewGuid()}@wavens.test";

            var staff = new Staff
            {
                Id = Guid.NewGuid(),
                FirstName = "Integration",
                LastName = "Test",
                ContactPhone = "85912345678",
                Email = email,
            };

            context.Add(staff);

            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var user = new User
            {
                Id = Guid.NewGuid(),
                StaffId = staff.Id,
                Email = staff.Email,
                UserName = staff.Email,
                EmailConfirmed = false,
                Status = UserStatus.Pending,
            };

            var createUserResult = await userManager.CreateAsync(user);

            Assert.True(createUserResult.Succeeded);

            var client = _factory.CreateClient();

            var generateInvitationResponse = await client.PostAsync(
                $"/api/invitations/{user.Id}",
                null,
                TestContext.Current.CancellationToken);

            var responseContent = await generateInvitationResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

            Assert.Single(mockEmailService.SentEmails);

            Assert.True(
                generateInvitationResponse.IsSuccessStatusCode,
                $"Status: {generateInvitationResponse.StatusCode}\nContent: {responseContent}"
                );
        }

        private static string ExtractInvitationTokenFromUrl(string content)
        {
            return content
                .Split(Environment.NewLine)
                .Single(line => line.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                .Trim();
        }

        [Fact]
        public async Task Invitation_AcceptInvitation_ShouldReturnOk()
        {
            using var scope = _factory.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<WavensDbContext>();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            var mockEmailService = scope.ServiceProvider.GetRequiredService<MockEmailService>();

            var email = $"invitation-{Guid.NewGuid()}@wavens.test";
            var password = "Invitation@123";

            var staff = new Staff
            {
                Id = Guid.NewGuid(),
                FirstName = "Integration",
                LastName = "Test",
                ContactPhone = "85912345678",
                Email = email,
            };

            context.Add(staff);

            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var user = new User
            {
                Id = Guid.NewGuid(),
                StaffId = staff.Id,
                Email = staff.Email,
                UserName = staff.Email,
                EmailConfirmed = false,
                Status = UserStatus.Pending,
            };

            var createUserResult = await userManager.CreateAsync(user);

            Assert.True(createUserResult.Succeeded);

            var client = _factory.CreateClient();

            var invitationToken = await userManager.GenerateUserTokenAsync(
                    user,
                    TokenOptions.DefaultProvider,
                    InvitationTokenPurpose
                    );

            var encodedToken = InvitationTokenCodec.Encode(invitationToken);

            var request = new AcceptInvitationRequest
            {
                UserId = user.Id,
                Password = password,
                Token = encodedToken
            };

            var acceptInvitationResponse = await client.PostAsJsonAsync(
               $"/api/invitations/accept",
               request,
               TestContext.Current.CancellationToken
               );

            context.ChangeTracker.Clear();

            var updatedUser = await context.Users.SingleAsync(
                u => u.Id == user.Id,
                TestContext.Current.CancellationToken
            );

            var validPassword = await userManager.CheckPasswordAsync(
                updatedUser,
                password
                );

            Assert.NotNull(updatedUser);
            Assert.Equal(HttpStatusCode.OK, acceptInvitationResponse.StatusCode);
            Assert.Equal(UserStatus.Active, updatedUser.Status);
            Assert.True(updatedUser.EmailConfirmed);
        }

        [Fact]
        public async Task Invitation_AcceptInvitationWhenTokenIsInvalid_ShouldReturnBadRequest()
        {
            using var scope = _factory.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<WavensDbContext>();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            var email = $"accept-inviation-{Guid.NewGuid()}@wavens.test";
            var password = "Test@123";

            var staff = new Staff
            {
                Id = Guid.NewGuid(),
                FirstName = "Integration",
                LastName = "Test",
                ContactPhone = "85912345678",
                Email = email,
            };

            context.Add(staff);

            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var user = new User
            {
                Id = Guid.NewGuid(),
                StaffId = staff.Id,
                Email = staff.Email,
                UserName = staff.Email,
                EmailConfirmed = false,
                Status = UserStatus.Pending,
            };

            var createUserResult = await userManager.CreateAsync(user, password);

            Assert.True(createUserResult.Succeeded);

            var client = _factory.CreateClient();

            var invitationToken = await userManager.GenerateUserTokenAsync(
                    user,
                    TokenOptions.DefaultProvider,
                    InvitationTokenPurpose
                    );

            var invalidToken = $"{invitationToken}-invalid-token";

            var encodedToken = InvitationTokenCodec.Encode(invalidToken);

            var request = new AcceptInvitationRequest
            {
                UserId = user.Id,
                Password = password,
                Token = encodedToken
            };

            var acceptInvitationResponse = await client.PostAsJsonAsync(
               $"/api/invitations/accept",
               request,
               TestContext.Current.CancellationToken
               );

            Assert.Equal(HttpStatusCode.BadRequest, acceptInvitationResponse.StatusCode);
        }
    }
}
