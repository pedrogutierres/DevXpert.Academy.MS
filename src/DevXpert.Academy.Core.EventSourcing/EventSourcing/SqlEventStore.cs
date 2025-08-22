using DevXpert.Academy.Core.Domain.DataModels;
using DevXpert.Academy.Core.Domain.DomainObjects;
using DevXpert.Academy.Core.Domain.Messages;
using DevXpert.Academy.Core.EventSourcing.EventStore.Repository;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace DevXpert.Academy.Core.EventSourcing.EventStore.EventSourcing
{
    public class SqlEventStore : IEventStore
    {
        private readonly IEventSourcingRepository _eventStoreRepository;
        private readonly IUser _user;

        public SqlEventStore(IEventSourcingRepository eventStoreRepository, IUser user)
        {
            _eventStoreRepository = eventStoreRepository;
            _user = user;
        }

        public Task SalvarEvento<T>(T evento) where T : Event
        {
            var serializedData = JsonConvert.SerializeObject(evento);

            var storedEvent = new StoredEvent(
                evento,
                serializedData,
                _user?.UsuarioIdNullValue(),
                _user?.Nome);

            return _eventStoreRepository.StoreAsync(storedEvent);
        }
    }
}