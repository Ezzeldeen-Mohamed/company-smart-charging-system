namespace company_smart_charging_system.Extensions
{
    public static class ExceptionHandlingExtensions
    {
        public static IServiceCollection AddCustomExceptionHandler(this IServiceCollection services)
        {
            services.AddExceptionHandler(options =>
            {
                options.ExceptionHandler = async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";

                    await context.Response.WriteAsync("{\"error\":\"Something went wrong.\"}");
                };
            });

            return services;
        }
    }
}
