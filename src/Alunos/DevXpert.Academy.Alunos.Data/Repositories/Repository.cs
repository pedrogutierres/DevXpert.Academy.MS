using DevXpert.Academy.Core.Domain.DataModels;
using DevXpert.Academy.Core.Domain.DomainObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DevXpert.Academy.Alunos.Data.Repositories
{
    public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity, IAggregateRoot
    {
        protected AlunosContext Db;
        protected DbSet<TEntity> DbSet;

        protected Repository(AlunosContext context)
        {
            Db = context;
            DbSet = Db.Set<TEntity>();
        }
        public IUnitOfWork UnitOfWork => Db;

        public virtual async Task Cadastrar(TEntity obj)
        {
            await DbSet.AddAsync(obj);
        }

        public virtual void Alterar(TEntity obj)
        {
            DbSet.Update(obj);
        }

        public virtual async Task Excluir(Guid id)
        {
            DbSet.Remove(await DbSet.FindAsync(id));
        }

        public virtual IQueryable<TEntity> Buscar(Expression<Func<TEntity, bool>> predicate, bool tracking = false)
        {
            return tracking
                   ? DbSet.Where(predicate)
                   : DbSet.AsNoTracking().Where(predicate);
        }

        public virtual Task<TEntity> ObterPorId(Guid id, bool tracking = false)
        {
            return tracking
                ? DbSet.FirstOrDefaultAsync(t => t.Id == id)
                : DbSet.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
        }

        public Task<bool> ExistePorId(Guid id)
        {
            return DbSet.AsNoTracking().AnyAsync(t => t.Id == id);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await Db.SaveChangesAsync();
        }

        public void Dispose()
        {
            Db.Dispose();
        }
    }
}
