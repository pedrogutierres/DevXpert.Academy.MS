using System;
using System.Collections.Generic;

namespace DevXpert.Academy.API.ViewModels.Cursos
{
    public class CursoViewModel
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; }
        public ConteudoProgramaticoViewModel ConteudoProgramatico { get; set; }
        public decimal Valor { get; set; }
        public bool Ativo { get; set; }
        public List<AulaViewModel> Aulas { get; set; }
    }

    public class ConteudoProgramaticoViewModel
    {
        public string Descricao { get; set; }
        public int CargaHoraria { get; set; }
    }

    public class AulaViewModel
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; }
        public string VideoUrl { get; set; }
    }
}
