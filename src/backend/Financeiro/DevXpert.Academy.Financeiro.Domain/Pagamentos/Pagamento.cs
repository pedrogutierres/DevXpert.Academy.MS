using DevXpert.Academy.Core.Domain.DomainObjects;
using DevXpert.Academy.Core.Domain.Messages.CommonMessages.IntegrationEvents;
using DevXpert.Academy.Financeiro.Domain.Alunos;
using DevXpert.Academy.Financeiro.Domain.Pagamentos.Validations;
using DevXpert.Academy.Financeiro.Domain.Pagamentos.ValuesObejcts;
using System;

namespace DevXpert.Academy.Financeiro.Domain.Pagamentos
{
    public class Pagamento : Entity, IAggregateRoot
    {
        public Guid MatriculaId { get; private set; }
        public decimal Valor { get; private set; }
        public DadosCartao DadosCartao { get; private set; }
        public PagamentoSituacao Situacao { get; private set; }

        // public List<PagamentoSituacao> HistoricoTransacoes { get; private set; } // Para uma próxima versão

        public virtual Matricula Matricula { get; private set; }

        private Pagamento() { }
        public Pagamento(Guid id, Guid matriculaId, decimal valor, DadosCartao dadosCartao)
        {
            Id = id;
            MatriculaId = matriculaId;
            Valor = valor;
            DadosCartao = dadosCartao;
            Situacao = new PagamentoSituacao(PagamentoSituacaoEnum.Pendente, DateTime.Now, "Pagamento pendente");
            //HistoricoTransacoes = [Situacao];

            AddEvent(new PagamentoRegistradoEvent(id, MatriculaId));
        }

        public void AprovarPagamento()
        {
            Situacao = new PagamentoSituacao(PagamentoSituacaoEnum.Aprovado, DateTime.Now, "Pagamento aprovado");
            //HistoricoTransacoes.Add(Situacao);

            AddEvent(new PagamentoAprovadoEvent(Id, MatriculaId));
        }

        public void RecusarPagamento(string motivo)
        {
            Situacao = new PagamentoSituacao(PagamentoSituacaoEnum.Recusado, DateTime.Now, motivo);
            //HistoricoTransacoes.Add(Situacao);

            AddEvent(new PagamentoRecusadoEvent(Id, MatriculaId, motivo));
        }

        public void EstornarPagamento(string motivo)
        {
            Situacao = new PagamentoSituacao(PagamentoSituacaoEnum.Estornado, DateTime.Now, motivo);
            //HistoricoTransacoes.Add(Situacao);

            AddEvent(new PagamentoEstornadoEvent(Id, MatriculaId, motivo));
        }

        public void CancelarPagamento(string motivo)
        {
            Situacao = new PagamentoSituacao(PagamentoSituacaoEnum.Cancelado, DateTime.Now, motivo);
            //HistoricoTransacoes.Add(Situacao);

            AddEvent(new PagamentoCanceladoEvent(Id, MatriculaId, motivo));
        }

        public override bool EhValido()
        {
            ValidationResult = new PagamentoEstaConsistenteValidation().Validate(this);

            return ValidationResult.IsValid;
        }
    }
}
