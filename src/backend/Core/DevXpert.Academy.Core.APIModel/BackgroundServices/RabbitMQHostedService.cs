using DevXpert.Academy.Core.Domain.Communication.Mediatr;
using DevXpert.Academy.Core.Domain.Messages;
using JsonNet.ContractResolvers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DevXpert.Academy.Core.APIModel.BackgroundServices
{
    public class RabbitMQOptions
    {
        public string BaseQueueName { get; set; }
        public Dictionary<string, Type> MessageTypes { get; set; } = [];
    }

    public sealed class RabbitMQHostedService : IHostedService, IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly IHost _host;
        private readonly ILogger<RabbitMQHostedService> _logger;
        private readonly ConnectionFactory _factory;

        private readonly string BaseQueueName;
        private readonly Dictionary<string, Type> MessageTypeRegistry;

        private IConnection _connection;
        private IModel _channel;

        private CancellationTokenSource _stoppingCts;

        public RabbitMQHostedService(IConfiguration configuration, IHost host, ILogger<RabbitMQHostedService> logger, RabbitMQOptions options)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _host = host ?? throw new ArgumentNullException(nameof(host));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            BaseQueueName = options.BaseQueueName ?? throw new ArgumentNullException(nameof(options.BaseQueueName));
            MessageTypeRegistry = options.MessageTypes ?? throw new ArgumentNullException(nameof(options.MessageTypes));

            _logger.LogInformation("RabbitMQ: Configurando...");

            _factory = new ConnectionFactory
            {
                Uri = new Uri(_configuration["RabbitMQ:Uri"])
            };

            _logger.LogInformation("RabbitMQ: Configurado");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("RabbitMQ: Iniciando...");

            _stoppingCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            var policy = Policy
                .Handle<BrokerUnreachableException>()
                .WaitAndRetryForeverAsync(retryAttempt => TimeSpan.FromSeconds(5), onRetry: (exception, timespan) =>
                {
                    _logger.LogError($"RabbitMQ: Falha ao conectar, erro: {exception}");
                });

            // Não utilizar await ou retornar a instancia task de policy
            // Pois se iniciar a aplicação aguardando essa task, nuncá iniciará a mesma se estiver OFF a fila
            policy.ExecuteAsync(() => ExecuteAsync(cancellationToken));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("RabbitMQ: Desligando...");

            _channel?.Close();
            _connection?.Close();

            _logger.LogInformation("RabbitMQ: Desligado");

            return Task.CompletedTask;
        }

        private Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("RabbitMQ: Connectando...");

            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();

            _logger.LogInformation("RabbitMQ: Connectado");

            if ((MessageTypeRegistry?.Count ?? 0) == 0)
            {
                _logger.LogInformation("RabbitMQ: Nenhuma mensagem configurada para ler da fila");
                return Task.CompletedTask;
            }

            // Criar a exchange automaticamente
            _channel.ExchangeDeclare("commands", ExchangeType.Direct, durable: true);

            foreach (var messageType in MessageTypeRegistry)
            {
                var key = $"{BaseQueueName}.{messageType.Key}";

                // Criar a fila automaticamente
                _channel.QueueDeclare(
                    queue: key,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );

                // Fazer o bind da fila com a exchange
                _channel.QueueBind(
                    queue: key,
                    exchange: "commands",
                    routingKey: messageType.Key
                );
            }

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (sender, eventArgs) =>
            {
                Command<bool> command;
                string contentString = null;

                try
                {
                    var props = eventArgs.BasicProperties;
                    if (!props?.IsHeadersPresent() ?? false)
                        throw new Exception("Nenhum header informado na mensagem.");

                    string messageTypeString = GetStringFromHeader(props, "MessageType");
                    var type = MessageTypeRegistry.GetValueOrDefault(messageTypeString) ?? throw new Exception($"Tipo da mensagem não identificado, MessageType: {messageTypeString}");

                    contentString = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

                    command = (Command<bool>)JsonConvert.DeserializeObject(Encoding.UTF8.GetString(eventArgs.Body.ToArray()), type, JsonSettingsPrivateSetters);
                }
                catch (JsonSerializationException ex)
                {
                    _logger.LogError($"Erro ao tentar desserializar o corpo da fila: {eventArgs.RoutingKey} | body: {contentString} | erro: {ex}");

                    // Aplicar o No Ack sem enfileirar novamente a mensagem pois ainda trará erro
                    _channel.BasicNack(eventArgs.DeliveryTag, false, false);

                    return;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Erro ao ler o corpo da fila: {eventArgs.RoutingKey} | body: {contentString} | erro: {ex}");

                    // TODO: verificar se deve jogar em uma dead queue

                    // Aplicar o No Ack sem enfileirar novamente a mensagem pois ainda trará erro
                    _channel.BasicNack(eventArgs.DeliveryTag, false, false);

                    return;
                }

                if (command == null)
                {
                    _logger.LogWarning($"Não foi identificado o Command da fila: {eventArgs.RoutingKey} | body: {contentString}");

                    // TODO: verificar se deve jogar em uma dead queue

                    // Aplicar o No Ack sem enfileirar novamente a mensagem pois ainda trará erro
                    _channel.BasicNack(eventArgs.DeliveryTag, false, false);
                    return;
                }

                //_logger.LogInformation($"Mensagem do tipo {command.GetType().FullName} recebida.");

                try
                {
                    using var scope = _host.Services.CreateScope();
                    var services = scope.ServiceProvider;
                    var mediator = services.GetRequiredService<IMediatorHandler>();

                    var result = await mediator.SendCommand(command, cancellationToken);

                    if (result)
                        _channel.BasicAck(eventArgs.DeliveryTag, false);
                    else
                    {
                        // TODO: verificar se deve jogar em uma dead queue

                        // Aplicar o No Ack sem enfileirar novamente a mensagem pois ainda trará erro
                        _channel.BasicNack(eventArgs.DeliveryTag, false, false);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Erro ao tentar chamar o Command {command.GetType().FullName} | fila: {eventArgs.RoutingKey} | body: {contentString} | erro: {ex}");

                    _channel.BasicNack(eventArgs.DeliveryTag, false, true);
                }
            };

            foreach (var messageType in MessageTypeRegistry)
            {
                var key = $"{BaseQueueName}.{messageType.Key}";

                _channel.BasicConsume(
                    queue: key,
                    autoAck: false,
                    consumer: consumer
                );
            }

            return Task.CompletedTask;
        }

        private static string GetStringFromHeader(IBasicProperties props, string key)
        {
            if (!props.Headers.TryGetValue(key, out var @byte))
                throw new Exception($"Header sem o {key} da mensagem.");

            var @string = Encoding.UTF8.GetString((byte[])@byte);
            if (string.IsNullOrEmpty(@string))
                throw new Exception($"Header com o {key} não identificável.");

            return @string;
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
            _stoppingCts?.Dispose();
        }

        private static readonly JsonSerializerSettings JsonSettingsPrivateSetters = new JsonSerializerSettings { ContractResolver = new PrivateSetterContractResolver() };
    }
}
