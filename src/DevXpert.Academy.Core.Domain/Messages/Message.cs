using MediatR;
using System;

namespace DevXpert.Academy.Core.Domain.Messages
{
    public abstract class Message<T> : IRequest<T>
    {
        public string MessageType { get; protected set; }
        public string AggregateRoot { get; protected set; }
        public Guid AggregateId { get; protected set; }

        protected Message()
        {
            MessageType = GetType().Name;
        }
    }
}
