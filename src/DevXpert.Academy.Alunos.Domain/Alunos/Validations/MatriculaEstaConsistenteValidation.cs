using DevXpert.Academy.Core.Domain.Validations;
using FluentValidation;

namespace DevXpert.Academy.Alunos.Domain.Alunos.Validations
{
    public class MatriculaEstaConsistenteValidation : DomainValidator<Matricula>
    {
        public MatriculaEstaConsistenteValidation()
        {
            ValidarId();
            ValidarAluno();
            ValidarCurso();
            ValidarConclusao();
        }

        private void ValidarAluno()
        {
            RuleFor(x => x.AlunoId)
                .NotEmpty().WithMessage("O aluno deve ser informado.");
        }
        private void ValidarCurso()
        {
            RuleFor(x => x.CursoId)
                .NotEmpty().WithMessage("O curso deve ser informado.");
        }

        private void ValidarConclusao()
        {
            When(p => p.Concluido, () =>
            {
                RuleFor(x => x.DataHoraConclusaoDoCurso)
                    .NotEmpty().WithMessage("A data de conclusão deve ser informada oara cursos concluídos.");

                RuleFor(x => x.Certificado)
                    .NotNull().WithMessage("O certificado deve ser gerado para cursos concluídos.");
            });

            When(p => !p.Concluido, () =>
            {
                RuleFor(x => x.DataHoraConclusaoDoCurso)
                    .Null().WithMessage("A data de conclusão não deve ser informada para cursos não concluídos.");
            }); 
        }
    }
}
