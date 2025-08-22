using DevXpert.Academy.Alunos.Domain.Alunos.Validations;
using DevXpert.Academy.Core.Domain.DomainObjects;
using System;

namespace DevXpert.Academy.Alunos.Domain.Alunos
{
    public class AulaConcluida : Entity
    {
        public Guid AlunoId { get; private set; }
        public Guid CursoId { get; private set; }
        public Guid AulaId { get; private set; }
        public DateTime DataHoraConclusao { get; private set; }

        public virtual Aluno Aluno { get; private set; }

        private AulaConcluida() { }
        public AulaConcluida(Guid alunoId, Guid cursoId, Guid aulaId, DateTime dataHoraConclusao)
        {
            AlunoId = alunoId;
            CursoId = cursoId;
            AulaId = aulaId;
            DataHoraConclusao = dataHoraConclusao;
        }

        public override bool EhValido()
        {
            ValidationResult = new AulaConcluidaEstaConsistenteValidation().Validate(this);

            return ValidationResult.IsValid;
        }
    }
}
