using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using EventSourcing.Abstractions;

namespace EventSourcing.Serialization.Json.Converters
{
    internal class EventStreamEntrySequenceConverter : JsonConverter<EventStreamEntrySequence>
    {
        public override EventStreamEntrySequence Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.GetUInt32();
        }

        public override void Write(Utf8JsonWriter writer, EventStreamEntrySequence value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }
}