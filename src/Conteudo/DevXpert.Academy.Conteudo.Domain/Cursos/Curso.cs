using DevXpert.Academy.Conteudo.Domain.Cursos.Adapters;
using DevXpert.Academy.Conteudo.Domain.Cursos.Events;
using DevXpert.Academy.Conteudo.Domain.Cursos.Validations;
using DevXpert.Academy.Conteudo.Domain.Cursos.ValuesObjects;
using DevXpert.Academy.Core.Domain.DomainObjects;
using DevXpert.Academy.Core.Domain.Messages.CommonMessages.Notifications;
using System;
using System.Collections.Generic;

namespace DevXpert.Academy.Conteudo.Domain.Cursos
{
    public sealed class Curso : Entity, IAggregateRoot
    {
        public string Titulo { get; private set; }
        public ConteudoProgramatico ConteudoProgramatico { get; private set; }
        public decimal Valor { get; private set; }
        public bool Ativo { get; private set; }

        public List<Aula> Aulas { get; private set; }

        private Curso() { }
        public Curso(Guid id, string titulo, ConteudoProgramatico conteudoProgramatico, decimal valor, List<Aula> aulas)
        {
            Id = id;
            Titulo = titulo;
            ConteudoProgramatico = conteudoProgramatico;
            Valor = valor;
            Aulas = aulas;
            Ativo = Aulas?.Count > 0;

            AddEvent(CursoAdapter.ToCursoCadastradoEvent(this));
        }

        public void AlterarTitulo(string titulo)
        {
            Titulo = titulo;

            AddEvent(new CursoTituloAlteradoEvent(Id, Titulo));
        }
        public void AlterarValor(decimal valor)
        {
            Valor = valor;

            AddEvent(new CursoValorAlteradoEvent(Id, Valor));
        }
        public void AlterarConteudoProgramatico(ConteudoProgramatico conteudoProgramatico)
        {
            ConteudoProgramatico = conteudoProgramatico;

            AddEvent(new CursoConteudoProgramaticoAlteradoEvent(Id, ConteudoProgramatico?.Descricao, ConteudoProgramatico?.CargaHoraria ?? 0));
        }
        public void Ativar()
        {
            Ativo = true;

            AddEvent(new CursoAtivadoEvent(Id));
        }
        public void Inativar()
        {
            Ativo = false;

            AddEvent(new CursoInativadoEvent(Id));
        }

        public void AdicionarAula(Aula aula)
        {
            Aulas ??= [];
            Aulas.Add(aula);

            AddEvent(new AulaCadastradaEvent(Id, aula.Id, aula.Titulo, aula.VideoUrl));
        }

        public void RemoverAula(Aula aula)
        {
            Aulas?.Remove(aula);

            if (Aulas?.Count == 0)
            {
                Inativar();
                AddEvent(new DomainNotification("RemoverAula", "O curso foi inativado pois não tem nenhuma aula mais.", false));
            }

            AddEvent(new AulaExcluidaEvent(Id, aula.Id));
        }

        public override bool EhValido()
        {
            ValidationResult = new CursoEstaConsistenteValidation().Validate(this);

            foreach (var aula in Aulas ?? [])
            {
                if (!aula.EhValido())
                    AdicionarValidationResultErros(aula.ValidationResult);
            }

            return ValidationResult.IsValid;
        }
    }
}
