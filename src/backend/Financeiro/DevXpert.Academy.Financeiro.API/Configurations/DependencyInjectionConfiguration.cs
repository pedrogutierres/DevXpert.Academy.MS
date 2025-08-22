using DevXpert.Academy.Core.Domain.Messages.CommonMessages.IntegrationEvents;
using DevXpert.Academy.Financeiro.AntiCorruption;
using DevXpert.Academy.Financeiro.Data.Repositories;
using DevXpert.Academy.Financeiro.Domain.Pagamentos.Commands;
using DevXpert.Academy.Financeiro.Domain.Pagamentos.Handlers;
using DevXpert.Academy.Financeiro.Domain.Pagamentos.Interfaces;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace DevXpert.Academy.Financeiro.API.Configurations
{
    public static class DependencyInjectionConfiguration
    {
        public static void AddDIConfiguration(this IServiceCollection services)
        {
            // DI / IoC - Alunos
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
        }
    }
}
