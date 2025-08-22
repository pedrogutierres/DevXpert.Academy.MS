using DevXpert.Academy.Core.Domain.Messages;
using System;

namespace DevXpert.Academy.Conteudo.Domain.Cursos.Events
{
    public sealed class CursoInativadoEvent : Event
    {
        public CursoInativadoEvent(Guid id) : base(nameof(Curso))
        {
            AggregateId = id;
        }
    }
}
