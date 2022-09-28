using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.Serialization.Abstractions;
using EventSourcing.Serialization.Json.Converters;

namespace EventSourcing.Serialization.Json
{
    internal class JsonSerializer : ISerializer
    {
        public static readonly JsonSerializerOptions DefaultJsonSerializerOptions = CreateDefaultJsonSerializerOptions(); 
        private static readonly SerializationFormat JsonSerializationFormat = SerializationFormat.Json;
        
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public SerializationFormat SerializationFormat => JsonSerializationFormat;
        
        public JsonSerializer(JsonSerializerOptions jsonSerializerOptions)
        {
            _jsonSerializerOptions = jsonSerializerOptions ?? throw new ArgumentNullException(nameof(jsonSerializerOptions));
        }

        public string Serialize(object @object)
        {
            return System.Text.Json.JsonSerializer.Serialize(@object, _jsonSerializerOptions);
        }

        public byte[] SerializeToUtf8Bytes(object @object)
        {
            return System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(@object, _jsonSerializerOptions);
        }

        public object Deserialize(string serializedObject, Type objectType)
        {
            return System.Text.Json.JsonSerializer.Deserialize(serializedObject, objectType, _jsonSerializerOptions);
        }

        public object DeserializeFromUtf8Bytes(byte[] serializedObject, Type objectType)
        {
            return System.Text.Json.JsonSerializer.Deserialize(serializedObject, objectType, _jsonSerializerOptions);
        }
        
        private static JsonSerializerOptions CreateDefaultJsonSerializerOptions()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new JsonStringEnumConverter());
            options.Converters.Add(new TypeConverter());

            return options;
        }
    }
}