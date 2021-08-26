using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using EventSourcing.Abstractions;

namespace EventSourcing.Serialization.Json.Converters
{
    internal class EventStreamEntryCausationIdConverter : JsonConverter<EventStreamEntryCausationId>
    {
        public override EventStreamEntryCausationId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.GetGuid();
        }

        public override void Write(Utf8JsonWriter writer, EventStreamEntryCausationId value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}