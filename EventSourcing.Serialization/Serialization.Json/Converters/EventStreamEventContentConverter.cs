using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using EventSourcing.Abstractions;
using EventSourcing.Abstractions.ValueObjects;

namespace EventSourcing.Serialization.Json.Converters
{
    internal class EventStreamEventContentConverter : JsonConverter<EventStreamEventContent>
    {
        public override EventStreamEventContent Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.GetString();
        }

        public override void Write(Utf8JsonWriter writer, EventStreamEventContent value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}