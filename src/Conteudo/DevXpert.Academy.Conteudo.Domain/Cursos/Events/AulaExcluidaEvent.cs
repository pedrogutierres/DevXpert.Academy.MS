using DevXpert.Academy.Core.Domain.Messages;
using System;

namespace DevXpert.Academy.Conteudo.Domain.Cursos.Events
{
    public sealed class AulaExcluidaEvent : Event
    {
        public Guid Id { get; private set; }

        public AulaExcluidaEvent(Guid aggregateId, Guid id) : base(nameof(Curso))
        {
            AggregateId = aggregateId;
            Id = id;
        }
    }
}
