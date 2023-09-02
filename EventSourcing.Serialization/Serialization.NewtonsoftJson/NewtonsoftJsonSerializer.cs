using System;
using System.Collections.Generic;
using System.Text;
using EventSourcing.ValueObjects;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace EventSourcing.Serialization.NewtonsoftJson
{
    internal class NewtonsoftJsonSerializer : ISerializer
    {
        public static readonly JsonSerializerSettings DefaultSerializerSettings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter>
            {
                new StringEnumConverter(),
                new IsoDateTimeConverter
                {
                    DateTimeFormat = "O"
                }
            }
        };

        private static readonly SerializationFormat JsonSerializationFormat = SerializationFormat.Json;

        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public SerializationFormat SerializationFormat => JsonSerializationFormat;

        public NewtonsoftJsonSerializer(JsonSerializerSettings jsonSerializerSettings)
        {
            _jsonSerializerSettings = jsonSerializerSettings ?? throw new ArgumentNullException(nameof(jsonSerializerSettings));
        }

        public string Serialize(object @object)
        {
            return JsonConvert.SerializeObject(@object, _jsonSerializerSettings);
        }

        public byte[] SerializeToUtf8Bytes(object @object)
        {
            var serializedObject = Serialize(@object);
            return Encoding.UTF8.GetBytes(serializedObject);
        }

        public object Deserialize(string serializedObject, Type objectType)
        {
            return JsonConvert.DeserializeObject(serializedObject, objectType, _jsonSerializerSettings);
        }

        public object DeserializeFromUtf8Bytes(byte[] serializedObject, Type objectType)
        {
            var serializedString = Encoding.UTF8.GetString(serializedObject);
            return Deserialize(serializedString, objectType);
        }
    }
}
