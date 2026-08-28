using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

using NewsflowApi.Data;
using NewsflowApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddNewsflowDatabase(builder.Configuration);
builder.Services.AddNewsflowIdentity();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();