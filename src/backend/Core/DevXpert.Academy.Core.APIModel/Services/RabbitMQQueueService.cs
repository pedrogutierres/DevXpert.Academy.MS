using DevXpert.Academy.Core.Domain.Communication;
using DevXpert.Academy.Core.Domain.Messages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DevXpert.Academy.Core.APIModel.Services
{
    public sealed class RabbitMQQueueService : IQueueService
    {
        private readonly ConnectionFactory _factory;
        private readonly IConfiguration _configuration;

        public RabbitMQQueueService(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            _factory = new ConnectionFactory
            {
                Uri = new Uri(_configuration["RabbitMQ:Uri"])
            };
        }

        public Task Enqueue(Command<bool> command)
        {
            using var connection = _factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare(
                exchange: "commands",
                type: ExchangeType.Direct,
                durable: true,
                autoDelete: false
            );

            var properties = channel.CreateBasicProperties();
            properties.Headers ??= new Dictionary<string, object>();
            properties.Headers.Add("MessageType", command.MessageType);

            var bodyMessage = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(command));

            channel.BasicPublish(exchange: "commands",
                                 routingKey: command.MessageType,
                                 basicProperties: properties,
                                 body: bodyMessage);

            return Task.CompletedTask;
        }
    }
}
