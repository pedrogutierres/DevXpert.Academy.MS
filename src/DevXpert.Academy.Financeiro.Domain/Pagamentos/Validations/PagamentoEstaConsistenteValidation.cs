using DevXpert.Academy.Core.Domain.Validations;
using FluentValidation;

namespace DevXpert.Academy.Financeiro.Domain.Pagamentos.Validations
{
    public class PagamentoEstaConsistenteValidation : DomainValidator<Pagamento>
    {
        public PagamentoEstaConsistenteValidation()
        {
            ValidarId();
            ValidarMatricula();
            ValidarValor();
            ValidarDadosCartao();
        }

        private void ValidarMatricula()
        {
            RuleFor(p => p.MatriculaId)
                .NotEmpty().WithMessage("A matrícula deve ser informada.");
        }
        private void ValidarValor()
        {
            RuleFor(p => p.Valor)
                .GreaterThan(0).WithMessage("O valor do pagamento deve ser maior que R$ 0,00.");
        }
        private void ValidarDadosCartao()
        {
            RuleFor(p => p.DadosCartao)
                .NotNull().WithMessage("Os dados do cartão para pagamento deve ser informado.");

            When(p => p.DadosCartao != null, () =>
            {
                RuleFor(p => p.DadosCartao.Token)
                    .NotEmpty().WithMessage("O token dos dados do cartão deve ser informado.");
            });
        }
    }
}
