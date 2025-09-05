using company_smart_charging_system;
using company_smart_charging_system.Extensions;
using company_smart_charging_system.Services;
using CompanySmartChargingSystem.Application.Services.IService;
using CompanySmartChargingSystem.Application.Services.Service;
using CompanySmartChargingSystem.Domain.ApplicationDbContext;
using CompanySmartChargingSystem.Domain.Entities;
using CompanySmartChargingSystem.Infrastructure;
using CompanySmartChargingSystem.Infrastructure.DataSeeding;
using CompanySmartChargingSystem.Infrastructure.JWT;
using CompanySmartChargingSystem.Infrastructure.Repositories;
using CompanySmartChargingSystem.Infrastructure.Repositories.IRepo;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Globalization;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add localization
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { new CultureInfo("en"), new CultureInfo("ar") };
    
    options.DefaultRequestCulture = new RequestCulture("ar");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

// Register custom localization service
builder.Services.AddScoped<ILocalizationService, LocalizationService>();

// Grouped extension methods
builder.Services.AddApiServices();
builder.Host.AddLoggingServices();
builder.Services.addDatabaseServices(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddMapperServices();
builder.Services.AddCachingServices();
builder.Services.AddCustomExceptionHandler();

builder.Services.AddScoped<IContractBackgroundService, ContractBackgroundService>();



builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(builder.Configuration.GetConnectionString("Connect")));


// Add your application services (repositories, unit of work, etc.)
builder.Services.addServicesAndRepos();


var app = builder.Build();

app.UseHangfireDashboard();

RecurringJob.AddOrUpdate<IContractBackgroundService>(
    "close-inactive-contracts",
    service => service.CloseInactiveContractsAsync(),
    Cron.Daily);

app.UseRequestLocalization();

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

// Test localization endpoint
app.MapGet("/api/test-localization", (ILocalizationService localizer) =>
{
    return new { 
        message = localizer.GetString("InvalidEmailOrPassword"),
        culture = System.Globalization.CultureInfo.CurrentUICulture.Name
    };
});

// Seed Identity data (roles and admin user)
await IdentityDataSeeder.SeedAsync(app.Services);

app.Run();
