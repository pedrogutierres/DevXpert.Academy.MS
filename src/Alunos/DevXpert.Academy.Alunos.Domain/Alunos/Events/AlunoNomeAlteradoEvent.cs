using DevXpert.Academy.Core.Domain.Messages;
using System;

namespace DevXpert.Academy.Alunos.Domain.Alunos.Events
{
    public sealed class AlunoNomeAlteradoEvent : Event
    {
        public string Nome { get; private set; }

        public AlunoNomeAlteradoEvent(Guid id, string nome) : base(nameof(Aluno))
        {
            AggregateId = id;
            Nome = nome;
        }
    }
}
