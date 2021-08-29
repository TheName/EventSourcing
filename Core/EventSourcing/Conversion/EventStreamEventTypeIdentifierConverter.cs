using System;
using System.Collections.Generic;
using System.Linq;
using EventSourcing.Abstractions;
using EventSourcing.Abstractions.Conversion;

namespace EventSourcing.Conversion
{
    internal class EventStreamEventTypeIdentifierConverter : IEventStreamEventTypeIdentifierConverter
    {
        private static readonly IReadOnlyDictionary<string, List<Type>> Types;
        
        static EventStreamEventTypeIdentifierConverter()
        {
            var validTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => !type.IsAbstract && !type.IsInterface);

            var dictionary = new Dictionary<string, List<Type>>();
            foreach (var type in validTypes)
            {
                var typeName = type.Name;
                if (!dictionary.ContainsKey(typeName))
                {
                    dictionary[typeName] = new List<Type>();
                }

                dictionary[typeName].Add(type);
            }

            Types = dictionary;
        }

        public EventStreamEventTypeIdentifier ToTypeIdentifier(Type type)
        {
            var identifier = type.Name;
            // verify type name is unique 
            GetType(identifier);

            return identifier;
        }

        public Type FromTypeIdentifier(EventStreamEventTypeIdentifier identifier)
        {
            return GetType((string) identifier);
        }

        private static Type GetType(string typeName)
        {
            if (!Types.ContainsKey(typeName))
            {
                throw new Exception($"Did not find type with name {typeName}");
            }

            if (Types[typeName].Count != 1)
            {
                throw new Exception(
                    $"There are too many types with name {typeName}. Types: {string.Join(", ", Types[typeName])}");
            }

            return Types[typeName][0];
        }
    }
}