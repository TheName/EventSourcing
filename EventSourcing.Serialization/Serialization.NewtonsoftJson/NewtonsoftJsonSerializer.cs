﻿using System;
using System.Collections.Generic;
using System.Text;
using EventSourcing.Serialization.Abstractions;
using EventSourcing.Serialization.NewtonsoftJson.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace EventSourcing.Serialization.NewtonsoftJson
{
    internal class NewtonsoftJsonSerializer : ISerializer
    {
        private static readonly JsonSerializerSettings DefaultSerializerSettings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter>
            {
                new StringEnumConverter(),
                new EventStreamEntryCausationIdConverter(),
                new EventStreamEntryCorrelationIdConverter(),
                new EventStreamEntryCreationTimeConverter(),
                new EventStreamEntryIdConverter(),
                new EventStreamEntrySequenceConverter(),
                new EventStreamEventContentConverter(),
                new EventStreamEventTypeIdentifierConverter(),
                new EventStreamIdConverter()
            }
        };

        public string Serialize(object @object)
        {
            return JsonConvert.SerializeObject(@object, DefaultSerializerSettings);
        }

        public byte[] SerializeToUtf8Bytes(object @object)
        {
            var serializedObject = Serialize(@object);
            return Encoding.UTF8.GetBytes(serializedObject);
        }

        public object Deserialize(string serializedObject, Type objectType)
        {
            return JsonConvert.DeserializeObject(serializedObject, objectType, DefaultSerializerSettings);
        }
    }
}