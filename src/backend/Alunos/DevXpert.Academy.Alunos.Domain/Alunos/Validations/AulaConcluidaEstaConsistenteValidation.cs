using DevXpert.Academy.Core.Domain.Validations;
using FluentValidation;

namespace DevXpert.Academy.Alunos.Domain.Alunos.Validations
{
    public class AulaConcluidaEstaConsistenteValidation : DomainValidator<AulaConcluida>
    {
        public AulaConcluidaEstaConsistenteValidation()
        {
            ValidarAlunoId();
            ValidarCursoId();
            ValidateAulaId();
        }

        private void ValidarAlunoId()
        {
            RuleFor(p => p.AlunoId)
                .NotEmpty().WithMessage("O aluno da aula concluída deve ser informado.");
        }

        private void ValidarCursoId()
        {
            RuleFor(p => p.CursoId)
                .NotEmpty().WithMessage("O curso da aula concluída deve ser informado.");
        }

        private void ValidateAulaId()
        {
            RuleFor(p => p.AulaId)
                .NotEmpty().WithMessage("A aula concluída deve ser informado.");
        }
    }
}
