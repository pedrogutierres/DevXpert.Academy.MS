using DevXpert.Academy.Core.Domain.Messages;
using System;

namespace DevXpert.Academy.Financeiro.Domain.Pagamentos.Commands
{
    public class SolicitarEstornoPagamentoDaMatriculaCommand : Command<bool>
    {
        public Guid MatriculaId => AggregateId;
        public string Motivo { get; private set; }

        public SolicitarEstornoPagamentoDaMatriculaCommand(Guid matriculaId, string motivo)
        {
            AggregateId = matriculaId;
            Motivo = motivo;
        }
    }
}
