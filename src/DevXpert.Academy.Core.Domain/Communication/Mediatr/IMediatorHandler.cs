using DevXpert.Academy.Core.Domain.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace DevXpert.Academy.Core.Domain.Communication.Mediatr
{
    public interface IMediatorHandler
    {
        Task<TResponse> SendCommand<TResponse>(Command<TResponse> command, CancellationToken cancellation = default);
        Task RaiseEvent<T>(T @event, CancellationToken cancellation = default) where T : Event;
    }
}
