using DevXpert.Academy.Core.Domain.DataModels;
using DevXpert.Academy.Core.EventSourcing.EventStore.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace DevXpert.Academy.Core.EventSourcing.EventStore.Repository
{
    public class EventSourcingRepository : IEventSourcingRepository
    {
        private readonly EventStoreSQLContext _context;

        public EventSourcingRepository(EventStoreSQLContext context)
        {
            _context = context;
        }

        public IList<StoredEvent> All(string aggregateRoot, Guid aggregateId)
        {
            return (from e in _context.StoredEvent where e.AggregateRoot == aggregateRoot && e.AggregateId == aggregateId select e).ToList();
        }

        public IList<StoredEvent> AllLast(string aggregateRoot, Guid aggregateId, int countLast = 10)
        {
            return (from e in _context.StoredEvent where e.AggregateRoot == aggregateRoot && e.AggregateId == aggregateId orderby e.Timestamp descending select e).Take(countLast).ToList();
        }

        public async Task StoreAsync(StoredEvent theEvent)
        {
            await _context.StoredEvent.AddAsync(theEvent);
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}