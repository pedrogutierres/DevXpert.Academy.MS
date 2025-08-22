using DevXpert.Academy.Alunos.Data.Repositories;
using DevXpert.Academy.Alunos.Domain.Alunos.Handlers;
using DevXpert.Academy.Alunos.Domain.Alunos.Interfaces;
using DevXpert.Academy.Alunos.Domain.Alunos.Services;
using DevXpert.Academy.Alunos.Domain.Cursos.Interfaces;
using DevXpert.Academy.API.Authentication;
using DevXpert.Academy.API.Models;
using DevXpert.Academy.Conteudo.Domain.Cursos.Interfaces;
using DevXpert.Academy.Conteudo.Domain.Cursos.Services;
using DevXpert.Academy.Core.Domain.Communication.Mediatr;
using DevXpert.Academy.Core.Domain.DomainObjects;
using DevXpert.Academy.Core.Domain.Messages.CommonMessages.IntegrationEvents;
using DevXpert.Academy.Core.Domain.Messages.CommonMessages.Notifications;
using DevXpert.Academy.Core.EventSourcing.EventStore.Context;
using DevXpert.Academy.Core.EventSourcing.EventStore.EventSourcing;
using DevXpert.Academy.Core.EventSourcing.EventStore.Repository;
using DevXpert.Academy.Financeiro.AntiCorruption;
using DevXpert.Academy.Financeiro.Data.Repositories;
using DevXpert.Academy.Financeiro.Domain.Pagamentos.Commands;
using DevXpert.Academy.Financeiro.Domain.Pagamentos.Handlers;
using DevXpert.Academy.Financeiro.Domain.Pagamentos.Interfaces;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace DevXpert.Academy.API.Configurations
{
    public static class DependencyInjectionConfiguration
    {
        public static void AddDIConfiguration(this IServiceCollection services)
        {
            // ASPNET
            services.AddHttpContextAccessor();

            // JWT
            //services.AddScoped<JwtTokenGenerate>();
            //services.AddScoped<AutenticacaoService>();

            // Domain Bus (Mediator)
            services.AddScoped<IMediatorHandler, MediatorHandler>();

            // Domain - Events
            services.AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>();

            // Infra - Data EventSourcing
            services.AddScoped<IEventSourcingRepository, EventSourcingRepository>();
            services.AddScoped<IEventStore, SqlEventStore>();
            services.AddScoped<EventStoreSQLContext>();

            // Infra - Identity
            services.AddScoped<IUser, AspNetUser>();
            services.AddScoped<AutenticacaoService>();

            // DI / IoC - Alunos
            services.AddScoped<IAlunoRepository, AlunoRepository>();
            services.AddScoped<IAlunoService, AlunoService>();
            services.AddScoped<ICursoReadOnlyRepository, CursoRepository>();

            // DI / IoC - Conteudo
            services.AddScoped<ICursoRepository, Conteudo.Data.Repositories.CursoRepository>();
            services.AddScoped<ICursoService, CursoService>();

            // DI / IoC - Financeiro
            services.AddScoped<IPagamentoRepository, PagamentoRepository>();
            services.AddScoped<IPagamentoCartaoCreditoFacade, PagamentoCartaoCreditoFacade>();
            services.AddScoped<IConfigurationManager, ConfigurationManager>();
            services.AddScoped<IPayPalGateway, PayPalGateway>();

            // Handlers
            services.AddScoped<IRequestHandler<RegistrarPagamentoCommand, bool>, PagamentoCommandHandler>();
            services.AddScoped<IRequestHandler<ProcessarPagamentoCommand, bool>, PagamentoCommandHandler>();
            services.AddScoped<IRequestHandler<SolicitarEstornoPagamentoDaMatriculaCommand, bool>, PagamentoCommandHandler>();
            services.AddScoped<IRequestHandler<EstornarPagamentoCommand, bool>, PagamentoCommandHandler>();

            // Events
            services.AddScoped<INotificationHandler<PagamentoRegistradoEvent>, PagamentoEventHandler>();
            services.AddScoped<INotificationHandler<PagamentoAprovadoEvent>, PagamentoEventHandler>();
            services.AddScoped<INotificationHandler<PagamentoRecusadoEvent>, PagamentoEventHandler>();
            services.AddScoped<INotificationHandler<PagamentoEstornadoEvent>, PagamentoEventHandler>();
            services.AddScoped<INotificationHandler<PagamentoCanceladoEvent>, PagamentoEventHandler>();
            services.AddScoped<INotificationHandler<PagamentoAprovadoEvent>, AlunoEventHandler>();
            services.AddScoped<INotificationHandler<PagamentoEstornadoEvent>, AlunoEventHandler>();
            services.AddScoped<INotificationHandler<PagamentoCanceladoEvent>, AlunoEventHandler>();
        }
    }
}
