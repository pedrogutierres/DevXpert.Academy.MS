using DevXpert.Academy.Core.Domain.Messages;
using System;
using System.Collections.Generic;

namespace DevXpert.Academy.Conteudo.Domain.Cursos.Events
{
    public sealed class CursoCadastradoEvent : Event
    {
        public string Titulo { get; private set; }
        public string Descricao { get; private set; }
        public int CargaHoraria { get; private set; }
        public decimal Valor { get; private set; }
        public List<AulaCadastradaEvent> Aulas { get; private set; }

        public CursoCadastradoEvent(Guid id, string titulo, string descricao, int cargaHorario, decimal valor, List<AulaCadastradaEvent> aulas) : base(nameof(Curso))
        {
            AggregateId = id;
            Titulo = titulo;
            Descricao = descricao;
            CargaHoraria = cargaHorario;
            Valor = valor;
            Aulas = aulas;
        }
    }
}
