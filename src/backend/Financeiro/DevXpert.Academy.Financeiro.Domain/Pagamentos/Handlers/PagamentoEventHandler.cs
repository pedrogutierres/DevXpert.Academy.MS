using DevXpert.Academy.Core.Domain.Communication.Mediatr;
using DevXpert.Academy.Core.Domain.Messages.CommonMessages.IntegrationEvents;
using DevXpert.Academy.Financeiro.Domain.Pagamentos.Commands;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace DevXpert.Academy.Financeiro.Domain.Pagamentos.Handlers
{
    public sealed class PagamentoEventHandler :
        INotificationHandler<PagamentoRegistradoEvent>,
        INotificationHandler<PagamentoAprovadoEvent>,
        INotificationHandler<PagamentoRecusadoEvent>,
        INotificationHandler<PagamentoCanceladoEvent>,
        INotificationHandler<PagamentoEstornadoEvent>
    {
        private readonly IMediatorHandler _mediator;

        public PagamentoEventHandler(IMediatorHandler mediator)
        {
            _mediator = mediator;
        }

        public async Task Handle(PagamentoRegistradoEvent notification, CancellationToken cancellationToken)
        {
            await _mediator.SendCommand(new ProcessarPagamentoCommand(notification.AggregateId, notification.MatriculaId), cancellationToken);
        }

        public Task Handle(PagamentoAprovadoEvent notification, CancellationToken cancellationToken)
        {
            // Enviar e-mail ao aluno informando que o pagamento foi aprovado e o curso está liberado

            return Task.CompletedTask;
        }

        public Task Handle(PagamentoRecusadoEvent notification, CancellationToken cancellationToken)
        {
            // Enviar e-mail ao aluno informando que o pagamento foi recusado e o curso está liberado

            return Task.CompletedTask;
        }

        public Task Handle(PagamentoCanceladoEvent notification, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task Handle(PagamentoEstornadoEvent notification, CancellationToken cancellationToken)
        {
            // Enviar e-mail ao aluno informando que o pagamento foi estornado

            return Task.CompletedTask;
        }
    }
}
