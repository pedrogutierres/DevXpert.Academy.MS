using DevXpert.Academy.Core.Domain.Messages;
using System;

namespace DevXpert.Academy.Alunos.Domain.IntegrationMessages
{
    public class PagamentoAprovadoEvent : Event
    {
        public Guid MatriculaId { get; private set; }

        public PagamentoAprovadoEvent(Guid id, Guid matriculaId) : base("Pagamento")
        {
            AggregateId = id;
            MatriculaId = matriculaId;
        }
    }
}
