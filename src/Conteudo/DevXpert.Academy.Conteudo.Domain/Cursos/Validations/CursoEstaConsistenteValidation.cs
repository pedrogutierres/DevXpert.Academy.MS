using DevXpert.Academy.Core.Domain.Validations;
using FluentValidation;

namespace DevXpert.Academy.Conteudo.Domain.Cursos.Validations
{
    public class CursoEstaConsistenteValidation : DomainValidator<Curso>
    {
        public CursoEstaConsistenteValidation()
        {
            ValidarId();
            ValidarTitulo();
            ValidarConteudoProgramatico();
            ValidarAulas();
        }

        private void ValidarTitulo()
        {
            RuleFor(p => p.Titulo)
                .NotEmpty().WithMessage("O título do curso deve ser informado.")
                .MaximumLength(100).WithMessage("O título do curso deve conter no máximo {MaxLength} caracteres.");
        }
        private void ValidarConteudoProgramatico()
        {
            RuleFor(p => p.ConteudoProgramatico)
                .NotNull().WithMessage(c => $"O conteúdo programático do curso {c.Titulo} deve ser informado.");

            When(p => p.ConteudoProgramatico != null, () =>
            {
                RuleFor(p => p.ConteudoProgramatico.Descricao)
                    .NotEmpty().WithMessage(c => $"A descrição do curso {c.Titulo} deve ser informada.");

                RuleFor(p => p.ConteudoProgramatico.CargaHoraria)
                    .InclusiveBetween(1, 1000).WithMessage(c => $"A carga horária do curso {c.Titulo} deve ser estar entre {{From}}h e {{To}}hs.");
            });
        }
        private void ValidarAulas()
        {
            When(p => p.Ativo, () =>
            {
                RuleFor(p => p.Aulas)
                    .NotEmpty().WithMessage(c => $"O curso {c.Titulo} deve ter aulas para ser ativado.");
            });
        }
    }
}
