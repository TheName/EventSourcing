using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using EventSourcing.Abstractions;

namespace EventSourcing.Serialization.Json.Converters
{
    internal class EventStreamEntryCreationTimeConverter : JsonConverter<EventStreamEntryCreationTime>
    {
        public override EventStreamEntryCreationTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.GetDateTime();
        }

        public override void Write(Utf8JsonWriter writer, EventStreamEntryCreationTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}