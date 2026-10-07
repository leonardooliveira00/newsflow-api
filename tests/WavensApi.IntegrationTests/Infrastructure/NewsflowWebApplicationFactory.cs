using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WavensApi.Infrastructure.Email;
using WavensApi.Infrastructure.Persistence;
using WavensApi.IntegrationTests.Infrastructure.Email;

namespace WavensApi.IntegrationTests.Infrastructure
{
    public sealed class WavensWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString) ||
                !connectionString.Contains("wavens_test"))
            {
                throw new InvalidOperationException(
                    "Integration tests must use the test database.");
            }

            var invitationTokenUrl = Environment.GetEnvironmentVariable("Frontend__InvitationTokenUrl");

            if (string.IsNullOrWhiteSpace(invitationTokenUrl))
            {
                throw new InvalidOperationException(
                    "Frontend__InvitationTokenUrl must be configured for integration tests.");
            }

            builder.ConfigureTestServices(services =>
            {
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                    typeof(IDbContextOptionsConfiguration<WavensDbContext>))
                    ?? throw new InvalidOperationException(
                        "WavensDbContext is not registered in the service collection.");

                services.Remove(dbContextDescriptor);

                services.AddDbContext<WavensDbContext>(options =>
                    options.UseNpgsql(connectionString));

                var emailServiceDescriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(IEmailService)) ?? throw new InvalidOperationException(
                        "IEmailService is not registered in the service collection.");

                services.Remove(emailServiceDescriptor);

                services.AddSingleton<MockEmailService>();

                services.AddSingleton<IEmailService>(
                    provider => provider.GetRequiredService<MockEmailService>());

                services.PostConfigure<SecurityStampValidatorOptions>(options =>
                {
                    options.ValidationInterval = TimeSpan.Zero;
                });
            });
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            return base.CreateHost(builder);
        }
    }
}
