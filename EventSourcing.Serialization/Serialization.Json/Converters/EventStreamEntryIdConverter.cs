using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using EventSourcing.Abstractions;
using EventSourcing.Abstractions.ValueObjects;

namespace EventSourcing.Serialization.Json.Converters
{
    internal class EventStreamEntryIdConverter : JsonConverter<EventStreamEntryId>
    {
        public override EventStreamEntryId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.GetGuid();
        }

        public override void Write(Utf8JsonWriter writer, EventStreamEntryId value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}