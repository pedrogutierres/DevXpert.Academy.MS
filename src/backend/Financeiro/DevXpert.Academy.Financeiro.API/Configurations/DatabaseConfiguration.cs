using DevXpert.Academy.Financeiro.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DevXpert.Academy.Financeiro.API.Configurations
{
    public static class DatabaseConfiguration
    {
        public static IServiceCollection AddDbContextConfig(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        {
            services.AddDbContext<FinanceiroContext>();

            /*services.AddDbContext<FinanceiroContext>(options =>
            {
                if (env.IsDevelopment())
                {
                    var connectionString = configuration.GetConnectionString("DefaultConnectionLite") ?? throw new InvalidOperationException("String de conexão 'DefaultConnectionLite' não encontrada para banco SQLite em ambiente de desenvolvimento.");
                    options.UseSqlite(connectionString);

                }
                else
                {
                    var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("String de conexão 'DefaultConnection' não encontrada.");
                    options.UseSqlServer(connectionString);
                }
            });*/

            return services;
        }
    }
}
