using DevXpert.Academy.Alunos.Domain.Alunos;
using DevXpert.Academy.Core.Domain.DomainObjects;
using System.Collections.Generic;

namespace DevXpert.Academy.Alunos.Domain.Cursos
{
    public sealed class Curso : ReadOnlyEntity, IAggregateRoot
    {
        public string Titulo { get; private set; }
        public decimal Valor { get; private set; }
        public bool Ativo { get; private set; }

        public List<Matricula> Matriculas { get; private set; }
        public List<Aula> Aulas { get; private set; }

        private Curso() { }
    }
}
