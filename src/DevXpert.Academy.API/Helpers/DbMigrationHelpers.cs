using Dapper;
using DevXpert.Academy.Alunos.Data;
using DevXpert.Academy.API.Authentication;
using DevXpert.Academy.Conteudo.Data;
using DevXpert.Academy.Core.EventSourcing.EventStore.Context;
using DevXpert.Academy.Financeiro.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace DevXpert.Academy.API.Helpers
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
            var conteudoContext = scope.ServiceProvider.GetRequiredService<ConteudoContext>();
            var alunosContext = scope.ServiceProvider.GetRequiredService<AlunosContext>();
            var financeiroContext = scope.ServiceProvider.GetRequiredService<FinanceiroContext>();
            var eventContext = scope.ServiceProvider.GetRequiredService<EventStoreSQLContext>();

            if (env.IsDevelopment() || env.EnvironmentName == "Test")
            {
                await applicationDbContext.Database.MigrateAsync();
                await conteudoContext.Database.MigrateAsync();
                await alunosContext.Database.MigrateAsync();
                await financeiroContext.Database.MigrateAsync();
                await eventContext.Database.MigrateAsync();

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
                Id = Guid.NewGuid().ToString(),
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

            await context.Database.GetDbConnection().ExecuteAsync(@"
                INSERT INTO Alunos (Id, Nome, DataHoraCriacao)
                            VALUES (@Id, @Nome, @DataHoraCriacao)",
                         new
                         {
                             Id = Guid.Parse(userAlunoPedro.Id),
                             Nome = "Pedro",
                             DataHoraCriacao = DateTime.UtcNow
                         });
            #endregion

            #region Aluno Eduardo
            var userAlunoEduardo = new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
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

            await context.Database.GetDbConnection().ExecuteAsync(@"
                INSERT INTO Alunos (Id, Nome, DataHoraCriacao)
                            VALUES (@Id, @Nome, @DataHoraCriacao)",
                        new
                        {
                            Id = Guid.Parse(userAlunoEduardo.Id),
                            Nome = "Eduardo",
                            DataHoraCriacao = DateTime.UtcNow
                        });
            #endregion

            #region Criar Cursos Exemplos
            for (int i = 1; i <= 3; i++)
            {
                var id = Guid.NewGuid();

                await context.Database.GetDbConnection().ExecuteAsync(@"
                    INSERT INTO Cursos (Id, Titulo, Descricao, CargaHoraria, Valor, Ativo, DataHoraCriacao)
                                VALUES (@Id, @Titulo, @Descricao, @CargaHoraria, @Valor, @Ativo, @DataHoraCriacao)",
                                new
                                {
                                    Id = id,
                                    Titulo = $"Curso {i}",
                                    Descricao = $"Descrição do curso {i}\n\nQuebra de linha teste",
                                    CargaHoraria = (i * 10),
                                    Valor = 100,
                                    Ativo = true,
                                    DataHoraCriacao = DateTime.UtcNow
                                });

                for (int a = 1; a <= new Random().Next(1, 10); a++)
                {
                    await context.Database.GetDbConnection().ExecuteAsync(@"
                        INSERT INTO CursosAulas (Id, CursoId, Titulo, VideoUrl, DataHoraCriacao)
                                         VALUES (@Id, @CursoId, @Titulo, @VideoUrl, @DataHoraCriacao)",
                                   new
                                   {
                                       Id = Guid.NewGuid(),
                                       CursoId = id,
                                       Titulo = $"Aula {i}",
                                       VideoUrl = $"https://www.youtube.com/watch?v={i}",
                                       DataHoraCriacao = DateTime.UtcNow
                                   });
                }
            }

            await context.Database.GetDbConnection().ExecuteAsync(@"
                INSERT INTO Cursos (Id, Titulo, Descricao, CargaHoraria, Valor, Ativo, DataHoraCriacao)
                            VALUES (@Id, @Titulo, @Descricao, @CargaHoraria, @Valor, @Ativo, @DataHoraCriacao)",
                           new
                           {
                               Id = Guid.NewGuid(),
                               Titulo = $"Curso Inativo",
                               Descricao = $"Descrição do curso inativo\n\nQuebra de linha teste",
                               CargaHoraria = 1,
                               Valor = 150,
                               Ativo = false,
                               DataHoraCriacao = DateTime.UtcNow
                           });
            #endregion

            await context.SaveChangesAsync();
        }
    }
}
