using DevXpert.Academy.Conteudo.Data.Repositories;
using DevXpert.Academy.Conteudo.Domain.Cursos.Interfaces;
using DevXpert.Academy.Conteudo.Domain.Cursos.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DevXpert.Academy.Conteudo.API.Configurations
{
    public static class DependencyInjectionConfiguration
    {
        public static void AddDIConfiguration(this IServiceCollection services)
        {
            // DI / IoC - Alunos
            services.AddScoped<ICursoRepository, CursoRepository>();
            services.AddScoped<ICursoService, CursoService>();

            // Handlers
            //services.AddScoped<IRequestHandler<RegistrarPagamentoCommand, bool>, PagamentoCommandHandler>();

            // Events
            //services.AddScoped<INotificationHandler<PagamentoAprovadoEvent>, AlunoEventHandler>();
        }
    }
}
