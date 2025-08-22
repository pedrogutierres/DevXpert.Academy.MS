using DevXpert.Academy.Financeiro.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace DevXpert.Academy.Financeiro.API.Helpers
{
    public static class DbMigrationHelperExtension
    {
        public static void UseDbMigrationHelper(this WebApplication app)
        {
            DbMigrationHelpers.EnsureSeedData(app).Wait();
        }
    }

    public static class DbMigrationHelpers
    {
        public static async Task EnsureSeedData(WebApplication serviceScope)
        {
            var services = serviceScope.Services.CreateScope().ServiceProvider;
            await EnsureSeedData(services);
        }

        public static async Task EnsureSeedData(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

            var financeiroContext = scope.ServiceProvider.GetRequiredService<FinanceiroContext>();

            if (env.IsDevelopment() || env.EnvironmentName == "Test")
            {
                await financeiroContext.Database.MigrateAsync();
                
                await EnsureSeed(scope);
            }
        }

        private static async Task EnsureSeed(IServiceScope scope)
        {
            // TODO: desenvolver
        }
    }
}
