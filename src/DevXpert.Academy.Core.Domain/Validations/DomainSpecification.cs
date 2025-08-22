using DevXpert.Academy.Core.Domain.DomainObjects;
using System.Threading.Tasks;

namespace DevXpert.Academy.Core.Domain.Validations
{
    public abstract class DomainSpecification<TEntity> where TEntity : Entity
    {
        protected readonly TEntity _entidade;

        protected DomainSpecification(TEntity entidade)
        {
            _entidade = entidade;
        }

        public abstract Task<bool> IsValidAsync();
    }
}
