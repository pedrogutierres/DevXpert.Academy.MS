using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DevXpert.Academy.Core.Domain.Messages.CommonMessages.Notifications
{
    public class DomainNotificationHandler : INotificationHandler<DomainNotification>
    {
        private List<DomainNotification> _notifications;
        private List<DomainNotification> _notificationsCache;

        public DomainNotificationHandler()
        { }

        public virtual List<DomainNotification> GetNotifications()
        {
            return _notifications?.Where(p => p.Error)?.ToList() ?? Enumerable.Empty<DomainNotification>().ToList();
        }
        public virtual List<DomainNotification> GetWarningNotifications()
        {
            return _notifications?.Where(p => !p.Error)?.ToList() ?? Enumerable.Empty<DomainNotification>().ToList();
        }

        public Task Handle(DomainNotification message, CancellationToken cancellationToken)
        {
            _notifications ??= new List<DomainNotification>();
            _notifications.Add(message);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Erro: {message.Key} - {message.Value}");

            return Task.CompletedTask;
        }
        
        public virtual bool HasNotifications()
        {
            return _notifications?.Any(p => p.Error) ?? false;
        }
        public virtual bool HasWarningNotifications()
        {
            return _notifications?.Any(p => !p.Error) ?? false;
        }
        public virtual bool HasKeyNotifications(string key)
        {
            return _notifications?.Any(p => p.Key == key) ?? false;
        }

        public virtual void ClearNotifications()
        {
            _notifications?.Clear();
        }

        public virtual void CreateCacheNotifications()
        {
            _notificationsCache = new List<DomainNotification>();
            _notifications?.ForEach(p => _notificationsCache.Add(p));
            _notifications?.Clear();
        }

        public virtual void RestoreCacheNotifications()
        {
            _notifications ??= new List<DomainNotification>();
            _notificationsCache?.ForEach(p => _notifications.Add(p));
            _notificationsCache?.Clear();
        }

        public void Dispose()
        {
            _notifications = new List<DomainNotification>();
            _notificationsCache = null;
        }
    }
}
