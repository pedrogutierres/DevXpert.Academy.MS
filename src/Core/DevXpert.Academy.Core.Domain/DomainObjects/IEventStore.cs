using DevXpert.Academy.Core.Domain.Messages;
using System.Threading.Tasks;

namespace DevXpert.Academy.Core.Domain.DomainObjects
{
    public interface IEventStore
    {
        Task SalvarEvento<T>(T evento) where T : Event;
    }
}
