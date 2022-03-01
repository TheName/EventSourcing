using System;
using EventSourcing.Abstractions.ValueObjects;
using Newtonsoft.Json;

namespace EventSourcing.Serialization.NewtonsoftJson.Converters
{
    internal class SerializationFormatConverter : JsonConverter<SerializationFormat>
    {
        public override void WriteJson(JsonWriter writer, SerializationFormat value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override SerializationFormat ReadJson(
            JsonReader reader,
            Type objectType,
            SerializationFormat existingValue,
            bool hasExistingValue, 
            JsonSerializer serializer)
        {
            var stringValue = (string) reader.Value;
            return stringValue;
        }
    }
}