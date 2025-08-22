using System;
using System.ComponentModel.DataAnnotations;

namespace DevXpert.Academy.API.ViewModels.Cursos
{
    public class AlterarCursoViewModel
    {
        [Required(ErrorMessage = "O título do curso deve ser informado.")]
        [MaxLength(100, ErrorMessage = "O título do curso deve conter no máximo {MaxLength} caracteres.")]
        public string Titulo { get; set; }

        public decimal Valor { get; set; }

        [Required(ErrorMessage = "O conteúdo programático do curso deve ser informado.")]
        public AlterarConteudoProgramaticoViewModel ConteudoProgramatico { get; set; }
    }

    public class AlterarConteudoProgramaticoViewModel
    {
        [Required(ErrorMessage = "A descrição do curso deve ser informada.")]
        public string Descricao { get; set; }

        [Range(1, 1000, ErrorMessage = "A carga horária do curso deve ser estar entre {1}h e {2}hs.")]
        public int CargaHoraria { get; set; }
    }
}
