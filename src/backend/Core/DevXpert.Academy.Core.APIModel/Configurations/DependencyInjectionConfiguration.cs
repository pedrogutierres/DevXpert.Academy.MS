using DevXpert.Academy.Core.APIModel.Models;
using DevXpert.Academy.Core.Domain.Communication.Mediatr;
using DevXpert.Academy.Core.Domain.DomainObjects;
using DevXpert.Academy.Core.Domain.Messages.CommonMessages.Notifications;
using DevXpert.Academy.Core.EventSourcing.EventStore.Context;
using DevXpert.Academy.Core.EventSourcing.EventStore.EventSourcing;
using DevXpert.Academy.Core.EventSourcing.EventStore.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace DevXpert.Academy.Core.APIModel.Configurations
{
    public static class DependencyInjectionConfiguration
    {
        public static void AddDIConfigurationDefault(this IServiceCollection services, IConfiguration configuration, IHostEnvironment env)
        {
            // ASPNET
            services.AddHttpContextAccessor();

            // Domain Bus (Mediator)
            services.AddScoped<IMediatorHandler, MediatorHandler>();

            // TODO: revisar depois o melhor lugar...
            // Infra - Data EventSourcing
            services.AddScoped<IEventSourcingRepository, EventSourcingRepository>();
            services.AddScoped<IEventStore, SqlEventStore>();
            services.AddScoped<EventStoreSQLContext>(); 
            services.AddDbContext<EventStoreSQLContext>(o =>
            {
                if (env.IsDevelopment() || env.EnvironmentName == "Test")
                {
                    var connectionString = configuration.GetConnectionString("DefaultConnectionLite") ?? throw new InvalidOperationException("String de conexão 'DefaultConnectionLite' não encontrada para banco SQLite em ambiente de desenvolvimento.");
                    o.UseSqlite(connectionString);

                }
                else
                {
                    var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("String de conexão 'DefaultConnection' não encontrada.");
                    o.UseSqlServer(connectionString);
                }
            });

            // Domain - Events
            services.AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>();

            // Infra - Identity
            services.AddScoped<IUser, AspNetUser>();
        }
    }
}
