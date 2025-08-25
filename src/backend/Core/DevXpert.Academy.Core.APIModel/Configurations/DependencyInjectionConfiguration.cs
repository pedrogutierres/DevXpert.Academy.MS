using DevXpert.Academy.Core.APIModel.Models;
using DevXpert.Academy.Core.Domain.Communication.Mediatr;
using DevXpert.Academy.Core.Domain.DomainObjects;
using DevXpert.Academy.Core.Domain.Messages.CommonMessages.Notifications;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

            // Domain - Events
            services.AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>();

            // Infra - Identity
            services.AddScoped<IUser, AspNetUser>();
        }
    }
}
