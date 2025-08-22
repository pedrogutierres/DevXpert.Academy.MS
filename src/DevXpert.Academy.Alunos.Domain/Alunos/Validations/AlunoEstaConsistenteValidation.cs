using DevXpert.Academy.Core.Domain.Validations;
using FluentValidation;

namespace DevXpert.Academy.Alunos.Domain.Alunos.Validations
{
    public class AlunoEstaConsistenteValidation : DomainValidator<Aluno>
    {
        public AlunoEstaConsistenteValidation()
        {
            ValidarId();
            ValidateNome();
        }

        private void ValidateNome()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("O nome do aluno deve ser informado.")
                .MaximumLength(200).WithMessage("O nome do aluno deve conter no máximo {MaxLength} caracteres.");
        }
    }
}
