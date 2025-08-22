
using DevXpert.Academy.Financeiro.Domain.Pagamentos;
using DevXpert.Academy.Financeiro.Domain.Pagamentos.Interfaces;

namespace DevXpert.Academy.Financeiro.AntiCorruption
{
    public class PagamentoCartaoCreditoFacade : IPagamentoCartaoCreditoFacade
    {
        private readonly IPayPalGateway _payPalGateway;
        private readonly IConfigurationManager _configManager;

        public PagamentoCartaoCreditoFacade(IPayPalGateway payPalGateway, IConfigurationManager configManager)
        {
            _payPalGateway = payPalGateway;
            _configManager = configManager;
        }

        public Pagamento ProcessarPagamento(Pagamento pagamento)
        {
            var apiKey = _configManager.GetValue("apiKey");
            var encriptionKey = _configManager.GetValue("encriptionKey");

            var serviceKey = _payPalGateway.GetPayPalServiceKey(apiKey, encriptionKey);
            var cardHashKey = _payPalGateway.GetCardHashKey(serviceKey, pagamento.DadosCartao.Token);

            var pagamentoResult = _payPalGateway.CommitTransaction(cardHashKey, pagamento.DadosCartao.Token == "-1" ? "-1" : pagamento.Id.ToString(), pagamento.Valor); // A regra -1 é para simular um pagamento recusado

            if (pagamentoResult)
                pagamento.AprovarPagamento();
            else
                pagamento.RecusarPagamento("Pagamento recusado pelo gateway PayPal");

            return pagamento;
        }

        public Pagamento EstornarPagamento(Pagamento pagamento, string motivo)
        {
            var apiKey = _configManager.GetValue("apiKey");
            var encriptionKey = _configManager.GetValue("encriptionKey");

            var estornoResult = _payPalGateway.RollbackTransaction(pagamento.DadosCartao.Token == "-1" ? "-1" : pagamento.Id.ToString()); // A regra -1 é para simular um pagamento recusado

            if (estornoResult)
                pagamento.EstornarPagamento(motivo ?? "Pagamento estornado");

            return pagamento;
        }
    }
}