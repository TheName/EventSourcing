using System;
using EventSourcing.Abstractions.Conversion;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.Serialization.Abstractions;

namespace EventSourcing.Conversion
{
    internal class EventStreamEventConverter : IEventStreamEventConverter
    {
        private readonly ISerializerProvider _serializerProvider;
        private readonly IEventStreamEventTypeIdentifierConverter _typeIdentifierConverter;

        public EventStreamEventConverter(
            ISerializerProvider serializerProvider,
            IEventStreamEventTypeIdentifierConverter typeIdentifierConverter)
        {
            _serializerProvider = serializerProvider ?? throw new ArgumentNullException(nameof(serializerProvider));
            _typeIdentifierConverter = typeIdentifierConverter ?? throw new ArgumentNullException(nameof(typeIdentifierConverter));
        }
        
        public EventStreamEventDescriptor ToEventDescriptor(object eventStreamEvent)
        {
            if (eventStreamEvent == null)
            {
                throw new ArgumentNullException(nameof(eventStreamEvent));
            }

            var typeIdentifier = _typeIdentifierConverter.ToTypeIdentifier(eventStreamEvent.GetType());
            var serializer = _serializerProvider.GetEventContentSerializer();
            var content = serializer.Serialize(eventStreamEvent);
            return new EventStreamEventDescriptor(content, serializer.SerializationFormat, typeIdentifier);
        }

        public object FromEventDescriptor(EventStreamEventDescriptor eventStreamEventDescriptor)
        {
            if (eventStreamEventDescriptor == null)
            {
                throw new ArgumentNullException(nameof(eventStreamEventDescriptor));
            }
            
            var eventType = _typeIdentifierConverter.FromTypeIdentifier(eventStreamEventDescriptor.EventTypeIdentifier);
            var serializer = _serializerProvider.GetSerializer(eventStreamEventDescriptor.EventContentSerializationFormat);
            return serializer.Deserialize(eventStreamEventDescriptor.EventContent, eventType);
        }
    }
}