using System;
using System.Collections.Generic;

namespace DevXpert.Academy.API.ViewModels.Alunos
{
    public class MeuPerfilViewModel
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public DateTime DataHoraCriacao { get; set; }
        public DateTime? DataHoraAlteracao { get; set; }

        public IEnumerable<MeuPerfilMatriculaViewModel> Matriculas { get; set; }
    }

    public class MeuPerfilMatriculaViewModel
    {
        public Guid Id { get; set; }
        public Guid CursoId { get; set; }
        public DateTime? DataHoraConclusao { get; set; }
        public bool Ativa { get; set; }
        public bool Concluido { get; set; }
        public DateTime DataHoraCriacao { get; set; }
        public DateTime? DataHoraAlteracao { get; set; }
        public MeuPerfilCertificadoViewModel Certificado { get; set; }
    }

    public class MeuPerfilCertificadoViewModel
    {
        public DateTime DataEmissao { get; set; }
    }
}
