using System;
using System.Collections.Generic;
using System.Linq;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.Serialization.Abstractions;

namespace EventSourcing.Serialization
{
    internal class SerializerProvider : ISerializerProvider
    {
        private readonly IEventSourcingSerializationConfiguration _serializationConfiguration;
        private readonly IReadOnlyDictionary<SerializationFormat, ISerializer> _serializers;

        public SerializerProvider(
            IEventSourcingSerializationConfiguration serializationConfiguration,
            IEnumerable<ISerializer> serializers)
        {
            _serializationConfiguration = serializationConfiguration ?? throw new ArgumentNullException(nameof(serializationConfiguration));
            if (serializers == null)
            {
                throw new ArgumentNullException(nameof(serializers));
            }
            
            _serializers = serializers.ToDictionary(serializer => serializer.SerializationFormat, serializer => serializer);
            if (_serializationConfiguration.EventContentSerializationFormat != null)
            {
                if (!_serializers.ContainsKey(_serializationConfiguration.EventContentSerializationFormat))
                {
                    throw new InvalidOperationException($"Could not find serializer with serialization format \"{_serializationConfiguration.EventContentSerializationFormat}\" which is configured as EventContentSerializationFormat");
                }
            }
            else
            {
                if (_serializers.Count != 1)
                {
                    throw new InvalidOperationException(
                        $"EventContentSerializationFormat configuration is missing and could not assume default format because the number of registered serializers is {_serializers.Count}");
                }
            }
        }
        
        public ISerializer GetEventContentSerializer()
        {
            if (_serializationConfiguration.EventContentSerializationFormat != null)
            {
                return GetSerializer(_serializationConfiguration.EventContentSerializationFormat);
            }

            return _serializers.Values.Single();
        }

        public ISerializer GetBusSerializer()
        {
            if (_serializationConfiguration.BusSerializationFormat != null)
            {
                return GetSerializer(_serializationConfiguration.BusSerializationFormat);
            }

            return _serializers.Values.Single();
        }

        public ISerializer GetForgettablePayloadSerializer()
        {
            if (_serializationConfiguration.ForgettablePayloadSerializationFormat != null)
            {
                return GetSerializer(_serializationConfiguration.ForgettablePayloadSerializationFormat);
            }

            return _serializers.Values.Single();
        }

        public ISerializer GetSerializer(SerializationFormat serializationFormat)
        {
            if (_serializers.TryGetValue(serializationFormat, out var serializer))
            {
                return serializer;
            }

            throw new InvalidOperationException(
                $"Could not find serializer with serialization format {serializationFormat}");
        }
    }
}