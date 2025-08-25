using DevXpert.Academy.Core.APIModel.BackgroundServices;
using DevXpert.Academy.Core.APIModel.Services;
using DevXpert.Academy.Core.Domain.Communication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DevXpert.Academy.Core.APIModel.Configurations
{
    public static class QueueConfiguration
    {
        public static IServiceCollection AddQueue(this IServiceCollection services, IConfiguration configuration, Func<RabbitMQOptions> options)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddSingleton<IQueueService, RabbitMQQueueService>();
            services.AddSingleton(options());
            services.AddHostedService<RabbitMQHostedService>();

            return services;
        }
    }
}
