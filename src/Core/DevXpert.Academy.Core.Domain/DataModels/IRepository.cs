using DevXpert.Academy.Core.Domain.DomainObjects;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DevXpert.Academy.Core.Domain.DataModels
{
    public interface IRepository<TEntity> : IDisposable where TEntity : Entity, IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; }

        Task Cadastrar(TEntity obj);
        void Alterar(TEntity obj);
        Task Excluir(Guid id);

        Task<TEntity> ObterPorId(Guid id, bool tracking = false);
        Task<bool> ExistePorId(Guid id);
        IQueryable<TEntity> Buscar(Expression<Func<TEntity, bool>> predicate, bool tracking = false);
    }
}
