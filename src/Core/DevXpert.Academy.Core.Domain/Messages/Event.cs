using MediatR;
using System;

namespace DevXpert.Academy.Core.Domain.Messages
{
    public abstract class Event : Message<bool>, INotification
    {
        public DateTime Timestamp { get; private set; }

        protected Event(string aggregateRoot)
        {
            AggregateRoot = aggregateRoot;
            Timestamp = DateTime.Now;
        }

        public void RegisterAggregateId(Guid id) => AggregateId = id;
    }
}
