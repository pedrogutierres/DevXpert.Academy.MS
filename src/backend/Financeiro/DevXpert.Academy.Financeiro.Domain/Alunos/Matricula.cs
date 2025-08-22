using DevXpert.Academy.Core.Domain.DomainObjects;
using DevXpert.Academy.Financeiro.Domain.Pagamentos;
using System;
using System.Collections.Generic;

namespace DevXpert.Academy.Financeiro.Domain.Alunos
{
    public class Matricula : ReadOnlyEntity
    {
        public Guid AlunoId { get; private set; }
        public bool Ativa { get; private set; }

        public virtual Aluno Aluno { get; private set; }
        public virtual IEnumerable<Pagamento> Pagamentos { get; private set; }

        private Matricula() { }
    }
}
