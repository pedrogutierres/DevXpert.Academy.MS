using DevXpert.Academy.Core.Domain.Messages;
using System;

namespace DevXpert.Academy.Core.Domain.DataModels
{
    public class StoredEvent : Event
    {
        public StoredEvent(Event evento, string data, Guid? user, string userName) : base("StoredEvent")
        {
            Id = Guid.NewGuid();
            AggregateRoot = evento.AggregateRoot;
            AggregateId = evento.AggregateId;
            MessageType = evento.MessageType;
            Data = data;
            User = user;
            UserName = userName;
        }

        private StoredEvent() : base("StoredEvent")
        { }

        public Guid Id { get; private set; }
        public string Data { get; private set; }
        public Guid? User { get; private set; }
        public string UserName { get; private set; }
    }
}
