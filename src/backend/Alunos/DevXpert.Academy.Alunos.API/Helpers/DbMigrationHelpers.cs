using Dapper;
using DevXpert.Academy.Alunos.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace DevXpert.Academy.Alunos.API.Helpers
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

            var context = scope.ServiceProvider.GetRequiredService<AlunosContext>();

            if (env.IsDevelopment() || env.EnvironmentName == "Test")
            {
                await context.Database.MigrateAsync();

                await EnsureSeed(scope, context);
            }
        }

        private static async Task EnsureSeed(IServiceScope scope, DbContext context)
        {
            if (await context.Database.GetDbConnection().ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Alunos") > 0)
                return;

            #region Aluno Pedro
            await context.Database.GetDbConnection().ExecuteAsync(@"
                INSERT INTO Alunos (Id, Nome, DataHoraCriacao)
                            VALUES (@Id, @Nome, @DataHoraCriacao)",
                         new
                         {
                             Id = Guid.Parse("6e91fbf4-73dd-444c-9f5c-f1821e568634"),
                             Nome = "Pedro",
                             DataHoraCriacao = DateTime.UtcNow
                         });
            #endregion

            #region Aluno Eduardo
            await context.Database.GetDbConnection().ExecuteAsync(@"
                INSERT INTO Alunos (Id, Nome, DataHoraCriacao)
                            VALUES (@Id, @Nome, @DataHoraCriacao)",
                        new
                        {
                            Id = Guid.Parse("b4c5b1e0-8a55-4c12-8903-51bea5c3756a"),
                            Nome = "Eduardo",
                            DataHoraCriacao = DateTime.UtcNow
                        });
            #endregion

            #region Criar Cursos Exemplos
            int i = 1;
            foreach (var id in new Guid[] { Guid.Parse("4caf0c97-f39c-49ee-bb2a-5b12c0a5e1c5"), Guid.Parse("7504a4d6-677b-4e70-afd2-420d8885e8bf"), Guid.Parse("02b3a6ea-f89a-4700-89ea-f2e0f13527ae") })
            {
                await context.Database.GetDbConnection().ExecuteAsync(@"
                    INSERT INTO Cursos (Id, Titulo, Valor, Ativo, DataHoraCriacao)
                                VALUES (@Id, @Titulo, @Valor, @Ativo, @DataHoraCriacao)",
                                new
                                {
                                    Id = id,
                                    Titulo = $"Curso {i}",
                                    Valor = 100,
                                    Ativo = true,
                                    DataHoraCriacao = DateTime.UtcNow
                                });

                for (int a = 1; a <= new Random().Next(1, 10); a++)
                {
                    await context.Database.GetDbConnection().ExecuteAsync(@"
                        INSERT INTO CursosAulas (Id, CursoId, DataHoraCriacao)
                                         VALUES (@Id, @CursoId, @DataHoraCriacao)",
                                   new
                                   {
                                       Id = Guid.NewGuid(),
                                       CursoId = id,
                                       DataHoraCriacao = DateTime.UtcNow
                                   });
                }

                i++;
            }

            await context.Database.GetDbConnection().ExecuteAsync(@"
                INSERT INTO Cursos (Id, Titulo, Valor, Ativo, DataHoraCriacao)
                            VALUES (@Id, @Titulo, @Valor, @Ativo, @DataHoraCriacao)",
                           new
                           {
                               Id = Guid.Parse("207b7f1c-fe91-435b-80b1-0d715952bf7b"),
                               Titulo = $"Curso Inativo",
                               Valor = 150,
                               Ativo = false,
                               DataHoraCriacao = DateTime.UtcNow
                           });
            #endregion

            await context.SaveChangesAsync();
        }
    }
}
