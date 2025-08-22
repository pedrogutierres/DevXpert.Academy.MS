using DevXpert.Academy.Core.Domain.Messages;
using System;

namespace DevXpert.Academy.Financeiro.Domain.Pagamentos.Commands
{
    public class RegistrarPagamentoCommand : Command<bool>
    {
        public Guid MatriculaId { get; private set; }
        public decimal Valor { get; private set; }
        public string DadosCartao_Nome { get; private set; }
        public string DadosCartao_Numero { get; private set; }
        public string DadosCartao_Vencimento { get; private set; }
        public string DadosCartao_CcvCvc { get; private set; }

        public RegistrarPagamentoCommand(Guid id, Guid matriculaId, decimal valor, string dadosCartao_Nome, string dadosCartao_Numero, string dadosCartao_Vencimento, string dadosCartao_CcvCvc)
        {
            AggregateId = id;
            MatriculaId = matriculaId;
            Valor = valor;
            DadosCartao_Nome = dadosCartao_Nome;
            DadosCartao_Numero = dadosCartao_Numero;
            DadosCartao_Vencimento = dadosCartao_Vencimento;
            DadosCartao_CcvCvc = dadosCartao_CcvCvc;
        }
    }
}
