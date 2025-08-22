using DevXpert.Academy.Core.Domain.DataModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevXpert.Academy.Core.EventSourcing.EventStore.Repository
{
    public interface IEventSourcingRepository : IDisposable
    {
        Task StoreAsync(StoredEvent theEvent);
        IList<StoredEvent> All(string aggregateRoot, Guid aggregateId);
        IList<StoredEvent> AllLast(string aggregateRoot, Guid aggregateId, int countLast = 10);
    }
}