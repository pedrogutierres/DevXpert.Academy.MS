namespace DevXpert.Academy.Financeiro.Domain.Pagamentos.Interfaces
{
    public interface IPagamentoCartaoCreditoFacade
    {
        Pagamento ProcessarPagamento(Pagamento pagamento);
        Pagamento EstornarPagamento(Pagamento pagamento, string motivo);
    }
}