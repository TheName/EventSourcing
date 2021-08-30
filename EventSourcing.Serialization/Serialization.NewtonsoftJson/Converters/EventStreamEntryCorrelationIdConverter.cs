using System;
using EventSourcing.Abstractions;
using EventSourcing.Abstractions.ValueObjects;

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