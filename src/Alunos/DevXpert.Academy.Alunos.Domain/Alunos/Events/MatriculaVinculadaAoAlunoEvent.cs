using DevXpert.Academy.Core.Domain.Messages;
using System;

namespace DevXpert.Academy.Alunos.Domain.Alunos.Events
{
    public sealed class MatriculaVinculadaAoAlunoEvent : Event
    {
        public Guid MatriculaId { get; private set; }
        public Guid CursoId { get; private set; }

        public MatriculaVinculadaAoAlunoEvent(Guid matriculaId, Guid alunoId, Guid cursoId) : base(nameof(Aluno))
        {
            AggregateId = alunoId;
            MatriculaId = matriculaId;
            CursoId = cursoId;
        }
    }
}
