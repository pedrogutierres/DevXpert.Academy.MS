using DevXpert.Academy.Conteudo.Domain.Cursos.Interfaces;
using DevXpert.Academy.Conteudo.Domain.Cursos.Validations.Specifications;
using DevXpert.Academy.Core.Domain.Validations;
using FluentValidation;

namespace DevXpert.Academy.Conteudo.Domain.Cursos.Validations
{
    public class CursoAptoParaAlterarValidation : DomainValidator<Curso>
    {
        private readonly ICursoRepository _cursoRepository;

        public CursoAptoParaAlterarValidation(ICursoRepository cursoRepository) : base()
        {
            _cursoRepository = cursoRepository;

            ValidarTituloDisponivel();
        }

        private void ValidarTituloDisponivel()
        {
            RuleFor(p => p.Titulo)
                .IsValidAsync(e => new CursoDeveTerTituloDisponivelSpecification(e, _cursoRepository))
                .WithMessage("Já existe um outro curso com o título informado.");
        }
    }
}
