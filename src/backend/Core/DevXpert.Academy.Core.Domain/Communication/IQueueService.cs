using DevXpert.Academy.Core.Domain.Messages;
using System.Threading.Tasks;

namespace DevXpert.Academy.Core.Domain.Communication
{
    public interface IQueueService
    {
        Task Enqueue(Command<bool> command);
    }
}
