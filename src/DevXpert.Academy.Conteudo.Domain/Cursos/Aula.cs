using DevXpert.Academy.Conteudo.Domain.Cursos.Validations;
using DevXpert.Academy.Core.Domain.DomainObjects;
using System;

namespace DevXpert.Academy.Conteudo.Domain.Cursos
{
    public sealed class Aula : Entity
    {
        public Guid CursoId { get; private set; }
        public string Titulo { get; private set; }
        public string VideoUrl { get; private set; }

        private Aula() { }
        public Aula(Guid id, Guid cursoId, string titulo, string videoUrl)
        {
            Id = id;
            CursoId = cursoId;
            Titulo = titulo;
            VideoUrl = videoUrl;
        }

        public override bool EhValido()
        {
            ValidationResult = new AulaEstaConsistenteValidation().Validate(this);

            return ValidationResult.IsValid;
        }
    }
}
