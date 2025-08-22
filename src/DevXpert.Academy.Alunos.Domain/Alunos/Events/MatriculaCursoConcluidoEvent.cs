using DevXpert.Academy.Core.Domain.Messages;
using System;

namespace DevXpert.Academy.Alunos.Domain.Alunos.Events
{
    public sealed class MatriculaCursoConcluidoEvent : Event
    {
        public Guid MatriculaId { get; private set; }
        public Guid CursoId { get; private set; }
        public DateTime DataHoraConclusao { get; private set; }

        public MatriculaCursoConcluidoEvent(Guid matriculaId, Guid alunoId, Guid cursoId, DateTime dataHoraConclusao) : base(nameof(Aluno))
        {
            AggregateId = alunoId;
            MatriculaId = matriculaId;
            CursoId = cursoId;
            DataHoraConclusao = dataHoraConclusao;
        }
    }
}
