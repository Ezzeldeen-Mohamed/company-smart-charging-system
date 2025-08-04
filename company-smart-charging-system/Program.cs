using CompanySmartChargingSystem.Domain.ApplicationDbContext;
using CompanySmartChargingSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.UseSerilog((context, loggerconfig) => loggerconfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddDbContext<AppDbContext>(x => x.UseSqlServer(builder.Configuration.GetConnectionString("Connect")));
builder.Services.AddIdentity<User, IdentityRole>(optin => {
    optin.Password.RequiredLength = 6;
    optin.Password.RequireLowercase = true;
    optin.Password.RequireUppercase = true;
    optin.Password.RequireNonAlphanumeric = false;
    optin.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<AppDbContext>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
