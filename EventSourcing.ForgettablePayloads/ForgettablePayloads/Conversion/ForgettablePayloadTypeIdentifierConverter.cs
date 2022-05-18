using System;
using System.Collections.Generic;
using System.Linq;
using EventSourcing.ForgettablePayloads.Abstractions.Conversion;
using EventSourcing.ForgettablePayloads.Abstractions.ValueObjects;

namespace EventSourcing.ForgettablePayloads.Conversion
{
    internal class ForgettablePayloadTypeIdentifierConverter : IForgettablePayloadTypeIdentifierConverter
    {
        private static readonly IReadOnlyDictionary<string, List<Type>> Types;

        static ForgettablePayloadTypeIdentifierConverter()
        {
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            var allReferencedAssemblies = loadedAssemblies
                .SelectMany(assembly => assembly.GetReferencedAssemblies())
                .Distinct()
                .Where(name => loadedAssemblies.All(assembly => assembly.FullName != name.FullName))
                .Select(name => AppDomain.CurrentDomain.Load(name))
                .Concat(loadedAssemblies);
            
            var validTypes = allReferencedAssemblies
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
        
        public ForgettablePayloadTypeIdentifierFormat TypeIdentifierFormat => ForgettablePayloadTypeIdentifierFormat.ClassName;
        
        public ForgettablePayloadTypeIdentifier ToTypeIdentifier(Type type)
        {
            var identifier = type.Name;
            // verify type name is unique 
            GetType(identifier);

            return identifier;
        }

        public Type FromTypeIdentifier(ForgettablePayloadTypeIdentifier identifier)
        {
            return GetType(identifier);
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