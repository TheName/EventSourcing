using System;
using EventSourcing.Abstractions;

namespace EventSourcing.Serialization.NewtonsoftJson.Converters
{
    internal class EventStreamEntryCausationIdConverter : GuidValueObjectConverter<EventStreamEntryCausationId>
    {
        protected override EventStreamEntryCausationId Cast(Guid value)
        {
            return value;
        }
    }
}