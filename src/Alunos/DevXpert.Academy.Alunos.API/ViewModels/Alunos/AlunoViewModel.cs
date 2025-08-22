using System;

namespace DevXpert.Academy.Alunos.API.ViewModels.Alunos
{
    public class AlunoViewModel
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public DateTime DataHoraCriacao { get; set; }
        public DateTime ?DataHoraAlteracao { get; set; }
    }

    public class MatriculaViewModel
    {
        public Guid Id { get; set; }
        public Guid AlunoId { get; set; }
        public Guid CursoId { get; set; }
        public DateTime? DataHoraConclusao { get; set; }
        public bool Ativa { get; set; }
        public bool Concluida { get; set; }
        public DateTime DataHoraCriacao { get; set; }
        public DateTime? DataHoraAlteracao { get; set; }
    }

    public class CertificadoViewModel
    {
        public DateTime DataEmissao { get; set; }
    }

    public class CursoViewModel
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataHoraCriacao { get; set; }
        public DateTime? DataHoraAlteracao { get; set; }
    }
}
