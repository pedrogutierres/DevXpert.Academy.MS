using DevXpert.Academy.Core.Domain.DataModels;
using DevXpert.Academy.Core.Domain.DomainObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DevXpert.Academy.Conteudo.Data.Repositories
{
    public abstract class ReadOnlyRepository<TEntity> : IReadOnlyRepository<TEntity> where TEntity : Entity, IEntity
    {
        protected ConteudoContext Db;
        protected DbSet<TEntity> DbSet;

        protected ReadOnlyRepository(ConteudoContext context)
        {
            Db = context;
            DbSet = Db.Set<TEntity>();
        }

        public virtual Task<TEntity> ObterPorId(Guid id)
        {
            return DbSet.AsNoTrackingWithIdentityResolution().FirstOrDefaultAsync(t => t.Id == id);
        }

        public Task<bool> ExistePorId(Guid id)
        {
            return DbSet.AsNoTrackingWithIdentityResolution().AnyAsync(t => t.Id == id);
        }

        public virtual IEnumerable<TEntity> Buscar(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.AsNoTrackingWithIdentityResolution().Where(predicate);
        }

        public void Dispose()
        {
            Db.Dispose();
        }

    }
}
