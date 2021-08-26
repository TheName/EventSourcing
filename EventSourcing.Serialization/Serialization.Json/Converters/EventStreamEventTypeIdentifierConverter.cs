using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using EventSourcing.Abstractions;

namespace EventSourcing.Serialization.Json.Converters
{
    internal class EventStreamEventTypeIdentifierConverter : JsonConverter<EventStreamEventTypeIdentifier>
    {
        public override EventStreamEventTypeIdentifier Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.GetString();
        }

        public override void Write(Utf8JsonWriter writer, EventStreamEventTypeIdentifier value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}