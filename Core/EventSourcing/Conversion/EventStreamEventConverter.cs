using System;
using System.Collections.Generic;
using EventSourcing.Abstractions.Conversion;
using EventSourcing.Abstractions.Hooks;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.Serialization.Abstractions;

namespace EventSourcing.Conversion
{
    internal class EventStreamEventConverter : IEventStreamEventConverter
    {
        private readonly ISerializerProvider _serializerProvider;
        private readonly IEnumerable<IEventStreamEventDescriptorPostDeserializationHook> _postDeserializationHooks;
        private readonly IEventStreamEventTypeIdentifierConverterProvider _typeIdentifierConverterProvider;

        public EventStreamEventConverter(
            ISerializerProvider serializerProvider,
            IEnumerable<IEventStreamEventDescriptorPostDeserializationHook> postDeserializationHooks,
            IEventStreamEventTypeIdentifierConverterProvider typeIdentifierConverterProvider)
        {
            _serializerProvider = serializerProvider ?? throw new ArgumentNullException(nameof(serializerProvider));
            _postDeserializationHooks = postDeserializationHooks ?? throw new ArgumentNullException(nameof(postDeserializationHooks));
            _typeIdentifierConverterProvider = typeIdentifierConverterProvider ?? throw new ArgumentNullException(nameof(typeIdentifierConverterProvider));
        }
        
        public EventStreamEventDescriptor ToEventDescriptor(object eventStreamEvent)
        {
            if (eventStreamEvent == null)
            {
                throw new ArgumentNullException(nameof(eventStreamEvent));
            }

            var typeIdentifierConverter = _typeIdentifierConverterProvider.GetEventTypeIdentifierConverter();
            var typeIdentifier = typeIdentifierConverter.ToTypeIdentifier(eventStreamEvent.GetType());
            var serializer = _serializerProvider.GetEventContentSerializer();
            var content = serializer.Serialize(eventStreamEvent);
            
            return new EventStreamEventDescriptor(
                content,
                serializer.SerializationFormat,
                typeIdentifier,
                typeIdentifierConverter.TypeIdentifierFormat);
        }

        public object FromEventDescriptor(EventStreamEventDescriptor eventStreamEventDescriptor)
        {
            if (eventStreamEventDescriptor == null)
            {
                throw new ArgumentNullException(nameof(eventStreamEventDescriptor));
            }
            
            var typeIdentifierConverter = _typeIdentifierConverterProvider.GetConverter(eventStreamEventDescriptor.EventTypeIdentifierFormat);
            var eventType = typeIdentifierConverter.FromTypeIdentifier(eventStreamEventDescriptor.EventTypeIdentifier);
            var serializer = _serializerProvider.GetSerializer(eventStreamEventDescriptor.EventContentSerializationFormat);
            var deserializedEvent = serializer.Deserialize(eventStreamEventDescriptor.EventContent, eventType);

            foreach (var eventStreamEventPostConversionModifier in _postDeserializationHooks)
            {
                eventStreamEventPostConversionModifier.PostEventDeserializationHook(deserializedEvent);
            }

            return deserializedEvent;
        }
    }
}