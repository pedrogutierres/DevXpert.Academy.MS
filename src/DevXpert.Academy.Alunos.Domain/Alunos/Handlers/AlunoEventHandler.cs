using DevXpert.Academy.Alunos.Domain.Alunos.Interfaces;
using DevXpert.Academy.Core.Domain.Messages.CommonMessages.IntegrationEvents;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace DevXpert.Academy.Alunos.Domain.Alunos.Handlers
{
    public class AlunoEventHandler
        : INotificationHandler<PagamentoAprovadoEvent>,
          INotificationHandler<PagamentoEstornadoEvent>,
          INotificationHandler<PagamentoCanceladoEvent>
    {
        private readonly IAlunoService _alunoService;

        public AlunoEventHandler(IAlunoService alunoService)
        {
            _alunoService = alunoService;
        }

        public async Task Handle(PagamentoAprovadoEvent notification, CancellationToken cancellationToken)
        {
            await _alunoService.AprovarMatricula(notification.MatriculaId);
        }

        public async Task Handle(PagamentoEstornadoEvent notification, CancellationToken cancellationToken)
        {
            await _alunoService.BloquearMatricula(notification.MatriculaId);
        }

        public async Task Handle(PagamentoCanceladoEvent notification, CancellationToken cancellationToken)
        {
            await _alunoService.BloquearMatricula(notification.MatriculaId);
        }
    }
}
