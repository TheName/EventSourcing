using System;
using EventSourcing.Abstractions;

namespace EventSourcing.Serialization.NewtonsoftJson.Converters
{
    internal class EventStreamEntryCorrelationIdConverter : GuidValueObjectConverter<EventStreamEntryCorrelationId>
    {
        protected override EventStreamEntryCorrelationId Cast(Guid value)
        {
            return value;
        }
    }
}