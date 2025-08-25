using Dapper;
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

            #region Aluno Pedro
            var userAlunoPedro = new IdentityUser
            {
                Id = Guid.Parse("6e91fbf4-73dd-444c-9f5c-f1821e568634").ToString(),
                UserName = "pedro@gmail.com",
                NormalizedUserName = "PEDRO@GMAIL.COM",
                Email = "PEDRO@GMAIL.COM",
                NormalizedEmail = "PEDRO@GMAIL.COM",
                AccessFailedCount = 0,
                LockoutEnabled = false,
                TwoFactorEnabled = false,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            result = await userManager.CreateAsync(userAlunoPedro, "Pedro@123456");

            if (!result.Succeeded)
                return;

            await userManager.AddToRoleAsync(userAlunoPedro, "Aluno");
            #endregion

            #region Aluno Eduardo
            var userAlunoEduardo = new IdentityUser
            {
                Id = Guid.Parse("b4c5b1e0-8a55-4c12-8903-51bea5c3756a").ToString(),
                UserName = "eduardo.pires@desenvolvedor.io",
                NormalizedUserName = "EDUARDO.PIRES@DESENVOLVEDOR.IO",
                Email = "EDUARDO.PIRES@DESENVOLVEDOR.IO",
                NormalizedEmail = "EDUARDO.PIRES@DESENVOLVEDOR.IO",
                AccessFailedCount = 0,
                LockoutEnabled = false,
                TwoFactorEnabled = false,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            result = await userManager.CreateAsync(userAlunoEduardo, "Eduardo@123456");

            if (!result.Succeeded)
                return;

            await userManager.AddToRoleAsync(userAlunoEduardo, "Aluno");
            #endregion

            await context.SaveChangesAsync();
        }
    }
}
