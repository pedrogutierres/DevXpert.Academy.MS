using System;

namespace DevXpert.Academy.Core.Domain.Messages.CommonMessages.Notifications
{
    public class DomainNotification : Event
    {
        public Guid DomainNotificationId { get; private set; }
        public string Key { get; private set; }
        public string Value { get; private set; }
        public int Version { get; private set; }
        public bool Error { get; private set; } = true;

        public DomainNotification(string key, string value) : base("DomainNotification")
        {
            DomainNotificationId = Guid.NewGuid();
            Key = key;
            Value = value;
            Version = 1;
        }
        public DomainNotification(string key, string value, bool error)
            : this(key, value)
        {
            Error = error;
        }

        public void ChangeValue(string value) => Value = value;
    }
}
