using System;

namespace DevXpert.Academy.Core.Domain.Messages.CommonMessages.IntegrationEvents
{
    public class PagamentoRegistradoEvent : Event
    {
        public Guid MatriculaId { get; private set; }

        public PagamentoRegistradoEvent(Guid id, Guid matriculaId) : base("Pagamento")
        {
            AggregateId = id;
            MatriculaId = matriculaId;
        }
    }
}
