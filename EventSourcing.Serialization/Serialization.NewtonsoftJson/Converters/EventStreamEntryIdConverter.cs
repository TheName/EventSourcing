using System;
using EventSourcing.Abstractions;

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