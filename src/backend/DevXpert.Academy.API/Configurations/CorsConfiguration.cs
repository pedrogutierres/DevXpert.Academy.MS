using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DevXpert.Academy.API.Configurations
{
    public static class CorsConfiguration
    {
        public static IServiceCollection AddCorsConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("Test", builder =>
                            builder
                                .WithOrigins(configuration.GetSection("AllowedHosts").Get<string>())
                                .AllowAnyMethod()
                                .AllowAnyHeader());

                options.AddPolicy("Development", builder =>
                            builder
                                .WithOrigins(configuration.GetSection("AllowedHosts").Get<string>())
                                .AllowAnyMethod()
                                .AllowAnyHeader());

                options.AddPolicy("Production", builder =>
                            builder
                                .WithOrigins(configuration.GetSection("AllowedHosts").Get<string>())
                                .SetIsOriginAllowedToAllowWildcardSubdomains()
                                .AllowAnyMethod()
                                .AllowAnyHeader());
            });

            return services;
        }
    }
}
