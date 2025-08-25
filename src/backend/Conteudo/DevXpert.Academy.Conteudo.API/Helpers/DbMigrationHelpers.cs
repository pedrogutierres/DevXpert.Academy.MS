using Dapper;
using DevXpert.Academy.Conteudo.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using System;
using System.Threading.Tasks;

namespace DevXpert.Academy.Conteudo.API.Helpers
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

            var context = scope.ServiceProvider.GetRequiredService<ConteudoContext>();

            if (env.IsDevelopment() || env.EnvironmentName == "Test")
            {
                await context.Database.MigrateAsync();
                
                await EnsureSeed(scope, context);
            }
        }

        private static async Task EnsureSeed(IServiceScope scope, DbContext context)
        {
            #region Criar Cursos Exemplos
            int i = 1;
            foreach (var id in new Guid[] { Guid.Parse("4caf0c97-f39c-49ee-bb2a-5b12c0a5e1c5"), Guid.Parse("7504a4d6-677b-4e70-afd2-420d8885e8bf"), Guid.Parse("02b3a6ea-f89a-4700-89ea-f2e0f13527ae") })
            {
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

                i++;
            }

            await context.Database.GetDbConnection().ExecuteAsync(@"
                INSERT INTO Cursos (Id, Titulo, Descricao, CargaHoraria, Valor, Ativo, DataHoraCriacao)
                            VALUES (@Id, @Titulo, @Descricao, @CargaHoraria, @Valor, @Ativo, @DataHoraCriacao)",
                           new
                           {
                               Id = Guid.Parse("207b7f1c-fe91-435b-80b1-0d715952bf7b"),
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
