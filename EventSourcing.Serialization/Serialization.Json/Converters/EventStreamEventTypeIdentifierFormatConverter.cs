using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using EventSourcing.Abstractions;
using EventSourcing.Abstractions.ValueObjects;

namespace EventSourcing.Serialization.Json.Converters
{
    internal class EventStreamEventTypeIdentifierFormatConverter : JsonConverter<EventStreamEventTypeIdentifierFormat>
    {
        public override EventStreamEventTypeIdentifierFormat Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.GetString();
        }

        public override void Write(Utf8JsonWriter writer, EventStreamEventTypeIdentifierFormat value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}