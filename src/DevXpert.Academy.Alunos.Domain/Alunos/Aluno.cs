using DevXpert.Academy.Alunos.Domain.Alunos.Events;
using DevXpert.Academy.Alunos.Domain.Alunos.Validations;
using DevXpert.Academy.Core.Domain.DomainObjects;
using DevXpert.Academy.Core.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DevXpert.Academy.Alunos.Domain.Alunos
{
    public sealed class Aluno : Entity, IAggregateRoot
    {
        public string Nome { get; private set; }

        public List<Matricula> Matriculas { get; private set; }
        public List<AulaConcluida> AulasConcluidas { get; private set; }

        private Aluno() { }
        public Aluno(Guid id, string nome)
        {
            Id = id;
            Nome = nome;

            AddEvent(new AlunoCadastradoEvent(Id, Nome));
        }

        public void AlterarNome(string nome)
        {
            Nome = nome;

            AddEvent(new AlunoNomeAlteradoEvent(Id, Nome));
        }

        public void Matricular(Matricula matricula)
        {
            if (EstaMatriculado(matricula.CursoId))
                throw new BusinessException("Você já está matriculado neste curso.");

            Matriculas ??= [];
            Matriculas.Add(matricula);

            AddEvent(new MatriculaVinculadaAoAlunoEvent(matricula.Id, Id, matricula.CursoId));
        }
        public bool EstaMatriculado(Guid cursoId) => Matriculas?.Any(p => p.CursoId == cursoId) ?? false;

        public void RegistrarAulaAssistida(Guid cursoId, Guid aulaId)
        {
            AulasConcluidas ??= [];

            var matricula = (Matriculas?.FirstOrDefault(p => p.CursoId == cursoId)) ?? throw new BusinessException("Matrícula não encontrada para o curso especificado.");

            if (AulasConcluidas.Any(a => a.AulaId == aulaId))
                return;

            AulasConcluidas.Add(new AulaConcluida(Id, cursoId, aulaId, DateTime.Now));

            AddEvent(new AulaConcluidaEvent(Id, matricula.Id, cursoId, aulaId, DateTime.Now));

            if (matricula.Curso?.Aulas?.Count == AulasConcluidas.Count)
                matricula.EmitirCertificado();
        }

        public override bool EhValido()
        {
            ValidationResult = new AlunoEstaConsistenteValidation().Validate(this);

            foreach (var matricula in Matriculas ?? [])
            {
                if (!matricula.EhValido())
                    AdicionarValidationResultErros(matricula.ValidationResult);
            }

            return ValidationResult.IsValid;
        }
    }
}
