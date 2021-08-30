using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using EventSourcing.Serialization.Abstractions;
using EventSourcing.Serialization.Json.Converters;

namespace EventSourcing.Serialization.Json
{
    internal class JsonSerializer : ISerializer
    {
        private static readonly JsonSerializerOptions JsonSerializerOptions;
        
        static JsonSerializer()
        {
            JsonSerializerOptions = new JsonSerializerOptions();
            JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            JsonSerializerOptions.Converters.Add(new EventStreamEntryCausationIdConverter());
            JsonSerializerOptions.Converters.Add(new EventStreamEntryCorrelationIdConverter());
            JsonSerializerOptions.Converters.Add(new EventStreamEntryCreationTimeConverter());
            JsonSerializerOptions.Converters.Add(new EventStreamEntryIdConverter());
            JsonSerializerOptions.Converters.Add(new EventStreamEntrySequenceConverter());
            JsonSerializerOptions.Converters.Add(new EventStreamEventContentConverter());
            JsonSerializerOptions.Converters.Add(new EventStreamEventTypeIdentifierConverter());
            JsonSerializerOptions.Converters.Add(new EventStreamIdConverter());
        }

        public string Serialize(object @object)
        {
            return System.Text.Json.JsonSerializer.Serialize(@object, JsonSerializerOptions);
        }

        public byte[] SerializeToUtf8Bytes(object @object)
        {
            return System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(@object, JsonSerializerOptions);
        }

        public object Deserialize(string serializedObject, Type objectType)
        {
            return System.Text.Json.JsonSerializer.Deserialize(serializedObject, objectType, JsonSerializerOptions);
        }

        public object DeserializeFromUtf8Bytes(byte[] serializedObject, Type objectType)
        {
            return System.Text.Json.JsonSerializer.Deserialize(serializedObject, objectType, JsonSerializerOptions);
        }
    }
}