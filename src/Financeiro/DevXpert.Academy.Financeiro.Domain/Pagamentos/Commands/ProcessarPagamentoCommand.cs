using DevXpert.Academy.Core.Domain.Messages;
using System;

namespace DevXpert.Academy.Financeiro.Domain.Pagamentos.Commands
{
    public class ProcessarPagamentoCommand : Command<bool>
    {
        public Guid MatriculaId { get; private set; }

        public ProcessarPagamentoCommand(Guid pagamentoId, Guid matriculaId)
        {
            AggregateId = pagamentoId;
            MatriculaId = matriculaId;
        }
    }
}
