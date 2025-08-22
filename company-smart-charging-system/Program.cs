using CompanySmartChargingSystem.Domain.ApplicationDbContext;
using CompanySmartChargingSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using CompanySmartChargingSystem.Application.Services.IService;
using CompanySmartChargingSystem.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Diagnostics;
using CompanySmartChargingSystem.Infrastructure.JWT;
using CompanySmartChargingSystem.Infrastructure.DataSeeding;
using CompanySmartChargingSystem.Application.Services.Service;

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

builder.Services.AddScoped(typeof(IBaseRepo<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IJWT, JWTRepo>();
builder.Services.AddScoped<IChargeTransactionService, ChargeTransactionService>();
builder.Services.Configure<JWTConfig>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddAutoMapper(typeof(CompanySmartChargingSystem.Application.DTOs.MappingProfile));

builder.Services.AddExceptionHandler(options =>
{
    options.ExceptionHandler = async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync("{\"error\":\"Something went wrong.\"}");
    };
});


// Configure JWT Authentication
var jwtConfig = builder.Configuration.GetSection("Jwt").Get<JWTConfig>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtConfig.Issuer,
        ValidAudience = jwtConfig.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Key))
    };
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlerMiddleware>();





// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Seed Identity data (roles and admin user)
await IdentityDataSeeder.SeedAsync(app.Services);

app.Run();
