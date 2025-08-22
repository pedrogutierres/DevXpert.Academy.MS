using DevXpert.Academy.Core.Domain.Messages;
using System;

namespace DevXpert.Academy.Alunos.Domain.Alunos.Events
{
    public sealed class AulaConcluidaEvent : Event
    {
        public Guid MatriculaId { get; private set; }
        public Guid CursoId { get; private set; }
        public Guid AulaId { get; private set; }
        public DateTime DataHoraConclusao { get; private set; }

        public AulaConcluidaEvent(Guid alunoId, Guid matriculaId,Guid cursoId, Guid aulaId, DateTime dataHoraConclusao) : base(nameof(Aluno))
        {
            AggregateId = alunoId;
            MatriculaId = matriculaId;
            CursoId = cursoId;
            AulaId = aulaId;
            DataHoraConclusao = dataHoraConclusao;
        }
    }
}
