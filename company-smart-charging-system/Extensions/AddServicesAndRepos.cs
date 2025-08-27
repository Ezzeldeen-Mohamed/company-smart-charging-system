using CompanySmartChargingSystem.Application.Services.IService;
using CompanySmartChargingSystem.Application.Services.Service;
using CompanySmartChargingSystem.Infrastructure;
using CompanySmartChargingSystem.Infrastructure.JWT;

namespace company_smart_charging_system.Extensions
{
    public static class AddServicesAndRepos
    {
        public static IServiceCollection addServicesAndRepos(this IServiceCollection services)
        {
            // Register Repositories
            services.AddScoped(typeof(IBaseRepo<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Register Services
            services.AddScoped<IJWT, JWTRepo>();
            services.AddScoped<IChargeTransactionService, ChargeTransactionService>();
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}
