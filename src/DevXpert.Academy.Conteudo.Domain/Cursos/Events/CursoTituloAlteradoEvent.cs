using DevXpert.Academy.Core.Domain.Messages;
using System;

namespace DevXpert.Academy.Conteudo.Domain.Cursos.Events
{
    public sealed class CursoTituloAlteradoEvent : Event
    {
        public string Titulo { get; private set; }

        public CursoTituloAlteradoEvent(Guid id, string titulo) : base(nameof(Curso))
        {
            AggregateId = id;
            Titulo = titulo;
        }
    }
}
