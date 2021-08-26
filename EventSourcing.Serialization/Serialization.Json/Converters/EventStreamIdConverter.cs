using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using EventSourcing.Abstractions;

namespace EventSourcing.Serialization.Json.Converters
{
    internal class EventStreamIdConverter : JsonConverter<EventStreamId>
    {
        public override EventStreamId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.GetGuid();
        }

        public override void Write(Utf8JsonWriter writer, EventStreamId value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}