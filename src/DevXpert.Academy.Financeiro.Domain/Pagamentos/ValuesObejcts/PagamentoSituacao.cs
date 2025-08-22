using System;

namespace DevXpert.Academy.Financeiro.Domain.Pagamentos.ValuesObejcts
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
}
