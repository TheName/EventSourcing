using System;

namespace Bus.RabbitMQ.IntegrationTests
{
    public class SimpleEvent
    {
        public Guid EventId { get; }

        public SimpleEvent(Guid eventId)
        {
            EventId = eventId;
        }
    }
}