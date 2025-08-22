using DevXpert.Academy.Auth.API.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using System;
using System.Threading.Tasks;

namespace DevXpert.Academy.Auth.API.Helpers
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

            var applicationDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (env.IsDevelopment() || env.EnvironmentName == "Test")
            {
                await applicationDbContext.Database.MigrateAsync();

                await EnsureSeed(scope, applicationDbContext);
            }
        }

        private static async Task EnsureSeed(IServiceScope scope, ApplicationDbContext context)
        {
            if (await context.Users.AnyAsync())
                return;

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            if (!await roleManager.RoleExistsAsync("Administrador"))
            {
                var role = new IdentityRole();
                role.Name = "Administrador";
                await roleManager.CreateAsync(role);
            }

            if (!await roleManager.RoleExistsAsync("Aluno"))
            {
                var role = new IdentityRole();
                role.Name = "Aluno";
                await roleManager.CreateAsync(role);
            }

            #region Criar Usuário Admin
            var userAdm = new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "admin@academy.com",
                NormalizedUserName = "ADMIN@ACADEMY.COM",
                Email = "ADMIN@ACADEMY.COM",
                NormalizedEmail = "ADMIN@ACADEMY.COM",
                AccessFailedCount = 0,
                LockoutEnabled = false,
                TwoFactorEnabled = false,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await userManager.CreateAsync(userAdm, "Academy@123456");

            if (!result.Succeeded)
                return;

            await userManager.AddToRoleAsync(userAdm, "Administrador");
            #endregion
        }
    }
}
