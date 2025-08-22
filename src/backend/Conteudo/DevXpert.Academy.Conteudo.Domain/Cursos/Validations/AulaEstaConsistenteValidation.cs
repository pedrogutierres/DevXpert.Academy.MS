using DevXpert.Academy.Core.Domain.Validations;
using FluentValidation;

namespace DevXpert.Academy.Conteudo.Domain.Cursos.Validations
{
    public class AulaEstaConsistenteValidation : DomainValidator<Aula>
    {
        public AulaEstaConsistenteValidation()
        {
            ValidarId();
            ValidarTitulo();
            ValidarVideoUrl();
        }

        private void ValidarTitulo()
        {
            RuleFor(p => p.Titulo)
                .NotEmpty().WithMessage("O título da aula deve ser informado.")
                .MaximumLength(100).WithMessage("O título da aula deve conter no máximo {MaxLength} caracteres.");
        }
        private void ValidarVideoUrl()
        {
            RuleFor(p => p.VideoUrl)
                .NotEmpty().WithMessage(c => $"A URL da vídeo aula {c.Titulo} deve ser informada.");
        }
    }
}
