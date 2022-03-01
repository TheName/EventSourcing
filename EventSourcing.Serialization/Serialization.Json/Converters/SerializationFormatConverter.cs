using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EventSourcing.Serialization.Json.Converters
{
    internal class SerializationFormatConverter : JsonConverter<EventSourcing.Abstractions.ValueObjects.SerializationFormat>
    {
        public override EventSourcing.Abstractions.ValueObjects.SerializationFormat Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.GetString();
        }

        public override void Write(Utf8JsonWriter writer, EventSourcing.Abstractions.ValueObjects.SerializationFormat value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}