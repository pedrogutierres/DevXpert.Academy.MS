using DevXpert.Academy.Core.Domain.Extensions;
using System;
using System.Text;

namespace DevXpert.Academy.Financeiro.Domain.Pagamentos.ValuesObejcts
{
    public sealed record DadosCartao
    {
        public string Token { get; private set; }

        private DadosCartao() { }
        public DadosCartao(string Nome, string Numero, string Vencimento, string CcvCvc)
        {
            // Regra apenas de exemplo, caso algum dos campos for informado o texto abaixo:
            // "RECUSADO", o token será "-1" para a facade de pagamento recusar o pagamento

            if ("RECUSADO".SwitchInline(Nome.ToUpper(), Numero.ToUpper(), Vencimento.ToUpper(), CcvCvc.ToUpper()))
                Token = "-1";
            else
                Token = TokenizarDadosCartao(Nome, Numero, Vencimento, CcvCvc);
        }

        private static string TokenizarDadosCartao(string Nome, string Numero, string Vencimento, string CcvCvc)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Nome}|{Numero}|{Vencimento}|{CcvCvc}"));
        }
    }
}
