using DevXpert.Academy.Core.Domain.Messages;
using System;

namespace DevXpert.Academy.Conteudo.Domain.Cursos.Events
{
    public sealed class CursoAtivadoEvent : Event
    {
        public CursoAtivadoEvent(Guid id) : base("Curso")
        {
            AggregateId = id;
        }
    }
}
