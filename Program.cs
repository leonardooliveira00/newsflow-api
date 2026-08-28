using NewsflowApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

using NewsflowApi.Domain.Users;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection String 'DefaultConnection' not found."
        );

builder.Services.AddDbContext<NewsflowDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddIdentityCore<User>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = true;

    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.AllowedForNewUsers = true;
}).AddEntityFrameworkStores<NewsflowDbContext>().AddSignInManager().AddDefaultTokenProviders();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();