using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DevXpert.Academy.API.ViewModels.Cursos
{
    public class CadastrarCursoViewModel
    {
        public Guid Id { get; } = Guid.NewGuid();

        [Required(ErrorMessage = "O título do curso deve ser informado.")]
        [MaxLength(100, ErrorMessage = "O título do curso deve conter no máximo {MaxLength} caracteres.")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "O conteúdo programático do curso deve ser informado.")]
        public CadastrarConteudoProgramaticoViewModel ConteudoProgramatico { get; set; }

        public decimal Valor { get; set; }

        public List<CadastrarAulaViewModel> Aulas { get; set; }
    }

    public class CadastrarConteudoProgramaticoViewModel
    {
        [Required(ErrorMessage = "A descrição do curso deve ser informada.")]
        public string Descricao { get; set; }

        [Range(1, 1000, ErrorMessage = "A carga horária do curso deve ser estar entre {1}h e {2}hs.")]
        public int CargaHoraria { get; set; }
    }

    public class CadastrarAulaViewModel
    {
        public Guid Id { get; } = Guid.NewGuid();

        [Required(ErrorMessage = "O título da aula deve ser informado.")]
        [MaxLength(100, ErrorMessage = "O título da aula deve conter no máximo {MaxLength} caracteres.")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "A URL da vídeo aula deve ser informada.")]
        public string VideoUrl { get; set; }
    }
}
