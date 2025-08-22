using System;

namespace DevXpert.Academy.Core.Domain.Messages.CommonMessages.IntegrationEvents
{
    public class PagamentoEstornadoEvent : Event
    {
        public Guid MatriculaId { get; private set; }
        public string Motivo { get; private set; }

        public PagamentoEstornadoEvent(Guid id, Guid matriculaId, string motivo) : base("Pagamento")
        {
            AggregateId = id;
            MatriculaId = matriculaId;
            Motivo = motivo;
        }
    }
}
