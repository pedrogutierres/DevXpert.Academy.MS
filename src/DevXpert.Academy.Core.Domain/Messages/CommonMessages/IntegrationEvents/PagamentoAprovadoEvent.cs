using System;

namespace DevXpert.Academy.Core.Domain.Messages.CommonMessages.IntegrationEvents
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
