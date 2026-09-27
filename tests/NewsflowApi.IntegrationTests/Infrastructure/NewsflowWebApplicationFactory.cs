using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NewsflowApi.Infrastructure.Email;
using NewsflowApi.Infrastructure.Persistence;
using NewsflowApi.IntegrationTests.Infrastructure.Email;
using System.Data;

namespace NewsflowApi.IntegrationTests.Infrastructure
{
    public sealed class NewsflowWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString) ||
                !connectionString.Contains("newsflow_test"))
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
                    typeof(IDbContextOptionsConfiguration<NewsflowDbContext>))
                    ?? throw new InvalidOperationException(
                        "NewsflowDbContext is not registered in the service collection.");

                services.Remove(dbContextDescriptor);

                services.AddDbContext<NewsflowDbContext>(options =>
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
            var host = base.CreateHost(builder);

            using var scope = host.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<NewsflowDbContext>();

            return host;
        }
    }
}
