using System;
using EventSourcing.Abstractions;
using EventSourcing.Abstractions.ValueObjects;

namespace EventSourcing.Serialization.NewtonsoftJson.Converters
{
    internal class EventStreamEntryIdConverter : GuidValueObjectConverter<EventStreamEntryId>
    {
        protected override EventStreamEntryId Cast(Guid value)
        {
            return value;
        }
    }
}