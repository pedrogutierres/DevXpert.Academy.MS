using DevXpert.Academy.Core.Domain.DomainObjects;
using System.Collections.Generic;

namespace DevXpert.Academy.Financeiro.Domain.Alunos
{
    public sealed class Aluno : ReadOnlyEntity, IAggregateRoot
    {
        public string Nome { get; private set; }

        public List<Matricula> Matriculas { get; private set; }

        private Aluno() { }
    }
}
