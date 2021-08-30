using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using EventSourcing.Abstractions;
using EventSourcing.Abstractions.ValueObjects;

namespace EventSourcing.Serialization.Json.Converters
{
    internal class EventStreamEntryCorrelationIdConverter : JsonConverter<EventStreamEntryCorrelationId>
    {
        public override EventStreamEntryCorrelationId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.GetGuid();
        }

        public override void Write(Utf8JsonWriter writer, EventStreamEntryCorrelationId value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}