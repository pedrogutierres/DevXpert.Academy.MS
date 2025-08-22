using DevXpert.Academy.Core.Domain.DomainObjects;
using DevXpert.Academy.Core.Domain.Messages;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DevXpert.Academy.Core.Domain.Communication.Mediatr
{
    public class MediatorHandler : IMediatorHandler
    {
        private readonly IMediator _mediator;
        private readonly IEventStore _eventStore;
        private readonly IServiceProvider _serviceProvider;

        public MediatorHandler(IMediator mediator, IEventStore eventStore, IServiceProvider serviceProvider)
        {
            _mediator = mediator;
            _eventStore = eventStore;
            _serviceProvider = serviceProvider;
        }

        public Task<TResponse> SendCommand<TResponse>(Command<TResponse> command, CancellationToken cancellation = default)
        {
            return _mediator.Send(command, cancellation);
        }

        public async Task RaiseEvent<T>(T @event, CancellationToken cancellation = default) where T : Event
        {
            if (!@event.MessageType.Equals("DomainNotification"))
                await _eventStore.SalvarEvento(@event);

            await _mediator.Publish(@event, cancellation);
        }

        public Task Enqueue(Command<bool> command, CancellationToken cancellation = default)
        {
            // A instancia do serviço de fila está sendo recuperado aqui pelo provider, para não ficar sendo criado atoa a todo momento
            // Até o momento são poucos serviços enfileirados
            var queueService = _serviceProvider.GetRequiredService<IQueueService>();

            return queueService.Enqueue(command);
        }
    }
}
