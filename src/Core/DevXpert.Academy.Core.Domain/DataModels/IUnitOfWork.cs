using System;
using System.Threading.Tasks;

namespace DevXpert.Academy.Core.Domain.DataModels
{
    public interface IUnitOfWork : IDisposable
    {
        Task<bool> CommitAsync();
        void DettachAllEntities();
    }
}
