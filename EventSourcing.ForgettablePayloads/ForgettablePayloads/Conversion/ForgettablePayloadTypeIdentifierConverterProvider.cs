using System;
using System.Collections.Generic;
using System.Linq;
using EventSourcing.ForgettablePayloads.Abstractions.Configurations;
using EventSourcing.ForgettablePayloads.Abstractions.Conversion;
using EventSourcing.ForgettablePayloads.Abstractions.ValueObjects;

namespace EventSourcing.ForgettablePayloads.Conversion
{
    internal class ForgettablePayloadTypeIdentifierConverterProvider : IForgettablePayloadTypeIdentifierConverterProvider
    {
        private readonly IForgettablePayloadTypeConversionConfiguration _configuration;
        private readonly IReadOnlyDictionary<ForgettablePayloadTypeIdentifierFormat, IForgettablePayloadTypeIdentifierConverter> _converters;

        public ForgettablePayloadTypeIdentifierConverterProvider(
            IForgettablePayloadTypeConversionConfiguration configuration,
            IEnumerable<IForgettablePayloadTypeIdentifierConverter> converters)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            
            if (converters == null)
            {
                throw new ArgumentNullException(nameof(converters));
            }

            var convertersDict = new Dictionary<ForgettablePayloadTypeIdentifierFormat, IForgettablePayloadTypeIdentifierConverter>();
            foreach (var convertersGroup in converters.GroupBy(converter => converter.TypeIdentifierFormat))
            {
                var typeIdentifierFormat = convertersGroup.Key;
                var typeIdentifierFormatConverters = convertersGroup.ToList();

                if (typeIdentifierFormatConverters.Count > 1)
                {
                    throw new ArgumentException(
                        $"Provided collection of {typeof(IForgettablePayloadTypeIdentifierConverter)} contains {typeIdentifierFormatConverters.Count} instances handling same type identifier format ({typeIdentifierFormat}). Please provide only one instance per type identifier format.",
                        nameof(converters));
                }
                
                convertersDict.Add(typeIdentifierFormat, typeIdentifierFormatConverters.Single());
            }

            _converters = convertersDict;
            if (_configuration.ForgettablePayloadTypeIdentifierFormat != null)
            {
                if (!_converters.ContainsKey(_configuration.ForgettablePayloadTypeIdentifierFormat))
                {
                    throw new InvalidOperationException($"Could not find converter with identifier format \"{_configuration.ForgettablePayloadTypeIdentifierFormat}\" which is configured as ForgettablePayloadTypeIdentifierFormat");
                }
            }
            else
            {
                if (_converters.Count != 1)
                {
                    throw new InvalidOperationException(
                        $"ForgettablePayloadTypeIdentifierFormat configuration is missing and could not assume default format because the number of registered converters is {_converters.Count}");
                }
            }
        }
        
        public IForgettablePayloadTypeIdentifierConverter GetForgettablePayloadTypeIdentifierConverter()
        {
            if (_configuration.ForgettablePayloadTypeIdentifierFormat != null)
            {
                return GetConverter(_configuration.ForgettablePayloadTypeIdentifierFormat);
            }
            
            return _converters.Values.Single();
        }

        public IForgettablePayloadTypeIdentifierConverter GetConverter(
            ForgettablePayloadTypeIdentifierFormat forgettablePayloadTypeIdentifierFormat)
        {
            if (_converters.TryGetValue(forgettablePayloadTypeIdentifierFormat, out var converter))
            {
                return converter;
            }

            throw new InvalidOperationException(
                $"Could not find converter with forgettable payload type identifier format {forgettablePayloadTypeIdentifierFormat}");
        }
    }
}