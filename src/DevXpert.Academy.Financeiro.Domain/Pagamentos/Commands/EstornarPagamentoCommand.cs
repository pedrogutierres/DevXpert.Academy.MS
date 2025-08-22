using DevXpert.Academy.Core.Domain.Messages;
using System;

namespace DevXpert.Academy.Financeiro.Domain.Pagamentos.Commands
{
    public class EstornarPagamentoCommand : Command<bool>
    {
        public string Motivo { get; private set; }

        public EstornarPagamentoCommand(Guid id, string motivo)
        {
            AggregateId = id;
            Motivo = motivo;
        }
    }
}
