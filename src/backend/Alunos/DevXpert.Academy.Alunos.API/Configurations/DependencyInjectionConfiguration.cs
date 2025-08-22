using DevXpert.Academy.Alunos.Data.Repositories;
using DevXpert.Academy.Alunos.Domain.Alunos.Handlers;
using DevXpert.Academy.Alunos.Domain.Alunos.Interfaces;
using DevXpert.Academy.Alunos.Domain.Alunos.Services;
using DevXpert.Academy.Alunos.Domain.Cursos.Interfaces;
using DevXpert.Academy.Alunos.Domain.IntegrationEvents;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace DevXpert.Academy.Alunos.API.Configurations
{
    public static class DependencyInjectionConfiguration
    {
        public static void AddDIConfiguration(this IServiceCollection services)
        {
            // DI / IoC - Alunos
            services.AddScoped<IAlunoRepository, AlunoRepository>();
            services.AddScoped<IAlunoService, AlunoService>();
            services.AddScoped<ICursoReadOnlyRepository, CursoRepository>();

            // Handlers
            //services.AddScoped<IRequestHandler<RegistrarPagamentoCommand, bool>, PagamentoCommandHandler>();

            // Events
            services.AddScoped<INotificationHandler<PagamentoAprovadoEvent>, AlunoEventHandler>();
            services.AddScoped<INotificationHandler<PagamentoEstornadoEvent>, AlunoEventHandler>();
            services.AddScoped<INotificationHandler<PagamentoCanceladoEvent>, AlunoEventHandler>();
        }
    }
}
