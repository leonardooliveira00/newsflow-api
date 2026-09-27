using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NewsflowApi.Application.Invitation;
using NewsflowApi.Domain.Entities.Identity.Users;
using NewsflowApi.Domain.Entities.Staffs;
using NewsflowApi.Domain.Enums.Identity.Users;
using NewsflowApi.Infrastructure.Email;
using NewsflowApi.Infrastructure.Persistence;
using NewsflowApi.IntegrationTests.Infrastructure;
using NewsflowApi.IntegrationTests.Infrastructure.Email;
using NewsflowApi.Presentation.Dtos.Requests.Authentication;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace NewsflowApi.IntegrationTests.Invitation
{
    public class InvitationTest(NewsflowWebApplicationFactory factory) : IClassFixture<NewsflowWebApplicationFactory>
    {
        private readonly NewsflowWebApplicationFactory _factory = factory;

        private const string InvitationTokenPurpose = "NewsflowInvitation";

        [Fact]
        public async Task Invitation_GenerateInvitationToken_ShouldReturnOk()
        {
            using var scope = _factory.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<NewsflowDbContext>();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            var mockEmailService = scope.ServiceProvider.GetRequiredService<MockEmailService>();

            var email = $"invitation-{Guid.NewGuid()}@newsflow.test";

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

            var emailService =
            scope.ServiceProvider.GetRequiredService<IEmailService>();

            Assert.Same(mockEmailService, emailService);

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

            Assert.Single(mockEmailService.SentEmails);
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

            var context = scope.ServiceProvider.GetRequiredService<NewsflowDbContext>();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            var mockEmailService = scope.ServiceProvider.GetRequiredService<MockEmailService>();

            var email = $"invitation-{Guid.NewGuid()}@newsflow.test";
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
    }
}
