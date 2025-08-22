using DevXpert.Academy.Alunos.Domain.Alunos.Events;
using DevXpert.Academy.Alunos.Domain.Alunos.Validations;
using DevXpert.Academy.Alunos.Domain.Alunos.ValuesObjects;
using DevXpert.Academy.Alunos.Domain.Cursos;
using DevXpert.Academy.Core.Domain.DomainObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DevXpert.Academy.Alunos.Domain.Alunos
{
    public class Matricula : Entity
    {
        public Guid AlunoId { get; private set; }
        public Guid CursoId { get; private set; }
        public DateTime? DataHoraConclusaoDoCurso { get; private set; }
        public bool Ativa { get; private set; }
        public bool Concluido { get; private set; }
        public Certificado Certificado { get; private set; }

        public virtual Aluno Aluno { get; private set; }
        public virtual Curso Curso { get; private set; }

        private Matricula() { }
        public Matricula(Guid id, Guid alunoId, Guid cursoId)
        {
            Id = id;
            AlunoId = alunoId;
            CursoId = cursoId;
            DataHoraCriacao = DateTime.Now;
            Certificado = null;
            Ativa = false;
            Concluido = false;

            AddEvent(new MatriculaCadastradaEvent(Id, AlunoId, CursoId));
        }

        public void Ativar()
        {
            Ativa = true;

            AddEvent(new MatriculaAtivadaEvent(Id, AlunoId, CursoId));
        }

        public void Bloquear()
        {
            Ativa = false;

            AddEvent(new MatriculaBloqueadaEvent(Id, AlunoId, CursoId));
        }

        public void EmitirCertificado()
        {
            Concluido = true;
            DataHoraConclusaoDoCurso = DateTime.Now;
            Certificado = new Certificado("gerar-link", DataHoraConclusaoDoCurso.Value);

            AddEvent(new MatriculaCursoConcluidoEvent(Id, AlunoId, CursoId, DataHoraConclusaoDoCurso.Value));
        }

        public override bool EhValido()
        {
            ValidationResult = new MatriculaEstaConsistenteValidation().Validate(this);

            return ValidationResult.IsValid;
        }
    }
}
