using DevXpert.Academy.Core.Domain.DomainObjects;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DevXpert.Academy.Core.Domain.DataModels
{
    public interface IReadOnlyRepository<TEntity> : IDisposable where TEntity : Entity, IEntity
    {
        Task<TEntity> ObterPorId(Guid id);
        Task<bool> ExistePorId(Guid id);
        IEnumerable<TEntity> Buscar(Expression<Func<TEntity, bool>> predicate);
    }
}
