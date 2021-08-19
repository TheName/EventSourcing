using System.Threading;
using System.Threading.Tasks;

namespace EventSourcing.Bus.RabbitMQ.Helpers
{
    internal interface IPublishingChannel
    {
        void Ack(ulong deliveryTag, bool multiple);
        void Nack(ulong deliveryTag, bool multiple);

        TaskCompletionSource<bool> WaitForAcknowledgment(ulong deliveryTag, CancellationToken cancellationToken);
    }
}