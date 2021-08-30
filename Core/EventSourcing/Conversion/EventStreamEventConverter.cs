using System;
using EventSourcing.Abstractions;
using EventSourcing.Abstractions.Conversion;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.Serialization.Abstractions;

namespace EventSourcing.Conversion
{
    internal class EventStreamEventConverter : IEventStreamEventConverter
    {
        private readonly ISerializer _serializer;
        private readonly IEventStreamEventTypeIdentifierConverter _typeIdentifierConverter;

        public EventStreamEventConverter(
            ISerializer serializer,
            IEventStreamEventTypeIdentifierConverter typeIdentifierConverter)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _typeIdentifierConverter = typeIdentifierConverter ?? throw new ArgumentNullException(nameof(typeIdentifierConverter));
        }
        
        public EventStreamEventDescriptor ToEventDescriptor(object eventStreamEvent)
        {
            if (eventStreamEvent == null)
            {
                throw new ArgumentNullException(nameof(eventStreamEvent));
            }

            var typeIdentifier = _typeIdentifierConverter.ToTypeIdentifier(eventStreamEvent.GetType());
            var content = _serializer.Serialize(eventStreamEvent);
            return new EventStreamEventDescriptor(content, typeIdentifier);
        }

        public object FromEventDescriptor(EventStreamEventDescriptor eventStreamEventDescriptor)
        {
            if (eventStreamEventDescriptor == null)
            {
                throw new ArgumentNullException(nameof(eventStreamEventDescriptor));
            }
            
            var eventType = _typeIdentifierConverter.FromTypeIdentifier(eventStreamEventDescriptor.EventTypeIdentifier);
            return _serializer.Deserialize(eventStreamEventDescriptor.EventContent, eventType);
        }
    }
}