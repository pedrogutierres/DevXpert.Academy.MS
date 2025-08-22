using DevXpert.Academy.Auth.API.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace DevXpert.Academy.Auth.API.Configurations
{
    public static class DatabaseConfiguration
    {
        public static IServiceCollection AddDbContextConfig(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        {
            services.AddDbContext<ApplicationDbContext>(o =>
            {
                if (env.IsDevelopment() || env.EnvironmentName == "Test")
                {
                    var connectionString = configuration.GetConnectionString("DefaultConnectionLite") ?? throw new InvalidOperationException("String de conexão 'DefaultConnectionLite' não encontrada para banco SQLite em ambiente de desenvolvimento.");
                    o.UseSqlite(connectionString);

                }
                else
                {
                    var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("String de conexão 'DefaultConnection' não encontrada.");
                    o.UseSqlServer(connectionString);
                }
            });

            return services;
        }
    }
}
