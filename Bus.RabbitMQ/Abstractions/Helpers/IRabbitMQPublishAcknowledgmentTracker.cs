using System;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client.Events;

namespace EventSourcing.Bus.RabbitMQ.Abstractions.Helpers
{
    internal interface IRabbitMQPublishAcknowledgmentTracker
    {
        EventHandler<BasicAckEventArgs> AckEventHandler { get; }
        
        EventHandler<BasicNackEventArgs> NackEventHandler { get; }

        TaskCompletionSource<bool> WaitForAcknowledgment(ulong deliveryTag, CancellationToken cancellationToken);
    }
}