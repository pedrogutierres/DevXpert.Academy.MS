using System;
using System.Collections.Generic;

namespace DevXpert.Academy.API.ViewModels.Cursos
{
    public class CursoAdmViewModel
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; }
        public ConteudoProgramaticoAdmViewModel ConteudoProgramatico { get; set; }
        public decimal Valor { get; set; }
        public bool Ativo { get; set; }
        public List<AulaAdmViewModel> Aulas { get; set; }
        public DateTime DataHoraCriacao { get; set; }
        public DateTime? DataHoraAlteracao { get; set; }
    }

    public class ConteudoProgramaticoAdmViewModel
    {
        public string Descricao { get; set; }
        public int CargaHoraria { get; set; }
    }

    public class AulaAdmViewModel
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; }
        public string VideoUrl { get; set; }
        public DateTime DataHoraCriacao { get; set; }
        public DateTime? DataHoraAlteracao { get; set; }
    }
}
