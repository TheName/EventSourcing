using System;
using System.Collections.Generic;
using System.Linq;
using EventSourcing.Configurations;
using EventSourcing.ValueObjects;

namespace EventSourcing.Conversion
{
    internal class EventStreamEventTypeIdentifierConverterProvider : IEventStreamEventTypeIdentifierConverterProvider
    {
        private readonly IEventSourcingTypeConversionConfiguration _typeConversionConfiguration;
        private readonly IReadOnlyDictionary<EventStreamEventTypeIdentifierFormat, IEventStreamEventTypeIdentifierConverter> _converters;
        
        public EventStreamEventTypeIdentifierConverterProvider(
            IEventSourcingTypeConversionConfiguration typeConversionConfiguration,
            IEnumerable<IEventStreamEventTypeIdentifierConverter> eventTypeIdentifierConverters)
        {
            _typeConversionConfiguration = typeConversionConfiguration ?? throw new ArgumentNullException(nameof(typeConversionConfiguration));
            
            if (eventTypeIdentifierConverters == null)
            {
                throw new ArgumentNullException(nameof(eventTypeIdentifierConverters));
            }

            var convertersDict = new Dictionary<EventStreamEventTypeIdentifierFormat, IEventStreamEventTypeIdentifierConverter>();
            foreach (var convertersGroup in eventTypeIdentifierConverters.GroupBy(converter => converter.TypeIdentifierFormat))
            {
                var typeIdentifierFormat = convertersGroup.Key;
                var typeIdentifierFormatConverters = convertersGroup.ToList();

                if (typeIdentifierFormatConverters.Count > 1)
                {
                    throw new ArgumentException(
                        $"Provided collection of {typeof(IEventStreamEventTypeIdentifierConverter)} contains {typeIdentifierFormatConverters.Count} instances handling same type identifier format ({typeIdentifierFormat}). Please provide only one instance per type identifier format.",
                        nameof(eventTypeIdentifierConverters));
                }
                
                convertersDict.Add(typeIdentifierFormat, typeIdentifierFormatConverters.Single());
            }

            _converters = convertersDict;
            if (_typeConversionConfiguration.EventTypeIdentifierFormat != null)
            {
                if (!_converters.ContainsKey(_typeConversionConfiguration.EventTypeIdentifierFormat))
                {
                    throw new InvalidOperationException($"Could not find converter with identifier format \"{_typeConversionConfiguration.EventTypeIdentifierFormat}\" which is configured as EventTypeIdentifierFormat");
                }
            }
            else
            {
                if (_converters.Count != 1)
                {
                    throw new InvalidOperationException(
                        $"EventTypeIdentifierFormat configuration is missing and could not assume default format because the number of registered converters is {_converters.Count}");
                }
            }
        }
        
        public IEventStreamEventTypeIdentifierConverter GetConverter(EventStreamEventTypeIdentifierFormat eventTypeIdentifierFormat)
        {
            if (_converters.TryGetValue(eventTypeIdentifierFormat, out var converter))
            {
                return converter;
            }

            throw new InvalidOperationException(
                $"Could not find converter with event type identifier format {eventTypeIdentifierFormat}");
        }

        public IEventStreamEventTypeIdentifierConverter GetEventTypeIdentifierConverter()
        {
            if (_typeConversionConfiguration.EventTypeIdentifierFormat != null)
            {
                return GetConverter(_typeConversionConfiguration.EventTypeIdentifierFormat);
            }
            
            return _converters.Values.Single();
        }
    }
}
