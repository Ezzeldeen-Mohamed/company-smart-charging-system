using CompanySmartChargingSystem.Application.DTOs;

namespace company_smart_charging_system.Extensions
{
    public static class MapperExtensions
    {
        public static IServiceCollection AddMapperServices(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfile));
            return services;
        }
    }
}
