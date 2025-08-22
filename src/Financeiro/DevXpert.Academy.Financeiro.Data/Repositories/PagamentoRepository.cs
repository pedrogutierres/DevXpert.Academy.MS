using DevXpert.Academy.Financeiro.Domain.Pagamentos;
using DevXpert.Academy.Financeiro.Domain.Pagamentos.Interfaces;
using System;
using System.Threading.Tasks;

namespace DevXpert.Academy.Financeiro.Data.Repositories
{
    public class PagamentoRepository : Repository<Pagamento>, IPagamentoRepository
    {
        public PagamentoRepository(FinanceiroContext context) : base(context)
        { }

        public override Task Excluir(Guid id) => throw new NotSupportedException("Exclusão de pagamento não suportada.");
    }
}
