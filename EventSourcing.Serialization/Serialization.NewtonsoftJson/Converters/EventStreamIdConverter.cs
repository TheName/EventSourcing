using System;
using EventSourcing.Abstractions;

namespace EventSourcing.Serialization.NewtonsoftJson.Converters
{
    internal class EventStreamIdConverter : GuidValueObjectConverter<EventStreamId>
    {
        protected override EventStreamId Cast(Guid value)
        {
            return value;
        }
    }
}