using System;

namespace DevXpert.Academy.Core.Domain.Messages
{
    public abstract class Command<T> : Message<T>
    {
        public DateTime Timestamp { get; private set; }

        protected Command()
        {
            Timestamp = DateTime.Now;
        }
    }
}
