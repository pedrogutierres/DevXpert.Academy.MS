using DevXpert.Academy.Core.Domain.Messages;
using System;

namespace DevXpert.Academy.Conteudo.Domain.Cursos.Events
{
    public sealed class AulaCadastradaEvent : Event
    {
        public Guid Id { get; private set; }
        public string Titulo { get; private set; }
        public string VideoUrl { get; private set; }

        public AulaCadastradaEvent(Guid aggregateId, Guid id, string titulo, string videoUrl) : base(nameof(Curso))
        {
            AggregateId = aggregateId;
            Id = id;
            Titulo = titulo;
            VideoUrl = videoUrl;
        }
    }
}
