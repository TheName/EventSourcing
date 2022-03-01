using System;
using System.Collections.Generic;
using System.Text;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.Serialization.Abstractions;
using EventSourcing.Serialization.NewtonsoftJson.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace EventSourcing.Serialization.NewtonsoftJson
{
    internal class NewtonsoftJsonSerializer : ISerializer
    {
        private static readonly JsonSerializerSettings DefaultSerializerSettings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter>
            {
                new StringEnumConverter(),
                new EventStreamEntryCausationIdConverter(),
                new EventStreamEntryCorrelationIdConverter(),
                new EventStreamEntryCreationTimeConverter(),
                new EventStreamEntryIdConverter(),
                new EventStreamEntrySequenceConverter(),
                new EventStreamEventContentConverter(),
                new EventStreamEventTypeIdentifierConverter(),
                new EventStreamIdConverter(),
                new SerializationFormatConverter()
            }
        };
        
        private static readonly SerializationFormat JsonSerializationFormat = SerializationFormat.Json;

        public SerializationFormat SerializationFormat => JsonSerializationFormat;

        public string Serialize(object @object)
        {
            return JsonConvert.SerializeObject(@object, DefaultSerializerSettings);
        }

        public byte[] SerializeToUtf8Bytes(object @object)
        {
            var serializedObject = Serialize(@object);
            return Encoding.UTF8.GetBytes(serializedObject);
        }

        public object Deserialize(string serializedObject, Type objectType)
        {
            return JsonConvert.DeserializeObject(serializedObject, objectType, DefaultSerializerSettings);
        }

        public object DeserializeFromUtf8Bytes(byte[] serializedObject, Type objectType)
        {
            var serializedString = Encoding.UTF8.GetString(serializedObject);
            return Deserialize(serializedString, objectType);
        }
    }
}