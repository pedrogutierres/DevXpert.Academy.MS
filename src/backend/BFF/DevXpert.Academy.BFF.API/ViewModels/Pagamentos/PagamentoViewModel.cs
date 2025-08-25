namespace DevXpert.Academy.BFF.API.ViewModels.Pagamentos
{
    public sealed record PagamentoSituacao(PagamentoSituacaoEnum Situacao, DateTime DataHoraProcessamento, string Mensagem);

    public enum PagamentoSituacaoEnum
    {
        Pendente,
        Aprovado,
        Recusado,
        Estornado,
        Cancelado,
    }

    public class PagamentoViewModel
    {
        public Guid Id { get; set; }
        public Guid MatriculaId { get; set; }
        public decimal Valor { get; set; }
        public PagamentoSituacao Situacao { get; set; }
        public DateTime DataHoraCriacao { get; set; }
        public DateTime DataHoraAlteracao { get; set; }
        public PagamentoMatriculaViewModel Matricula { get; set; }
    }

    public class PagamentoSituacaoViewModel
    {
        public PagamentoSituacaoEnum Situacao { get; set; }
        public DateTime DataHoraProcessamento { get; set; }
        public string Mensagem { get; set; }
    }

    public class PagamentoMatriculaViewModel
    {
        public Guid Id { get; set; }
        public bool Ativa { get; set; }
        public PagamentoAlunoViewModel Aluno { get; set; }

    }

    public class PagamentoAlunoViewModel
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
    }
}
