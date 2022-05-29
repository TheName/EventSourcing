using System;
using System.Collections.Generic;
using System.Linq;
using EventSourcing.Abstractions.Configurations;
using EventSourcing.Abstractions.Conversion;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.Conversion;
using Moq;
using TestHelpers.Attributes;
using Xunit;

namespace EventSourcing.UnitTests.Conversion
{
    public class EventStreamEventTypeIdentifierConverterProvider_Should
    {
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_Creating_WithNullConfiguration(
            IEnumerable<IEventStreamEventTypeIdentifierConverter> converters)
        {
            Assert.Throws<ArgumentNullException>(() =>
                new EventStreamEventTypeIdentifierConverterProvider(null, converters));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_Creating_WithNullConverters(
            IEventSourcingTypeConversionConfiguration configuration)
        {
            Assert.Throws<ArgumentNullException>(() =>
                new EventStreamEventTypeIdentifierConverterProvider(configuration, null));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentException_When_Creating_WithNotNullParameters_And_ThereAreMultipleConvertersReturningSameTypeIdentifierFormat(
            IEventSourcingTypeConversionConfiguration configuration,
            IEnumerable<IEventStreamEventTypeIdentifierConverter> converters,
            EventStreamEventTypeIdentifierFormat typeIdentifierFormat)
        {
            var converterMocksReturningSameTypeIdentifierFormat = Enumerable.Range(0, 3)
                .Select(i => new Mock<IEventStreamEventTypeIdentifierConverter>())
                .ToList();

            converterMocksReturningSameTypeIdentifierFormat
                .ForEach(mock => mock
                    .SetupGet(converter => converter.TypeIdentifierFormat)
                    .Returns(typeIdentifierFormat));

            var allConverters = new List<IEventStreamEventTypeIdentifierConverter>(converters);
            allConverters.AddRange(converterMocksReturningSameTypeIdentifierFormat.Select(mock => mock.Object));

            Assert.Throws<ArgumentException>(() =>
                new EventStreamEventTypeIdentifierConverterProvider(configuration, allConverters));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_InvalidOperationException_When_Creating_WithNotNullParameters_And_ConfigurationReturnsNotNullForgettablePayloadTypeIdentifierFormat_And_ThereIsNoConverterProvidedThatHandlesThisFormat(
            Mock<IEventSourcingTypeConversionConfiguration> configurationMock,
            List<IEventStreamEventTypeIdentifierConverter> converters,
            EventStreamEventTypeIdentifierFormat typeIdentifierFormat)
        {
            configurationMock
                .SetupGet(configuration => configuration.EventTypeIdentifierFormat)
                .Returns(typeIdentifierFormat);
            
            Assert.True(converters.All(converter => converter.TypeIdentifierFormat != typeIdentifierFormat));
            
            Assert.Throws<InvalidOperationException>(() =>
                new EventStreamEventTypeIdentifierConverterProvider(configurationMock.Object, converters));
        }
        
        [Theory]
        [AutoMoqData]
        public void NotThrow_When_Creating_WithNotNullParameters_And_ConfigurationReturnsNotNullForgettablePayloadTypeIdentifierFormat_And_ThereIsAConverterProvidedThatHandlesThisFormat(
            Mock<IEventSourcingTypeConversionConfiguration> configurationMock,
            List<IEventStreamEventTypeIdentifierConverter> converters,
            EventStreamEventTypeIdentifierFormat typeIdentifierFormat)
        {
            configurationMock
                .SetupGet(configuration => configuration.EventTypeIdentifierFormat)
                .Returns(typeIdentifierFormat);

            var requiredConverterMock = new Mock<IEventStreamEventTypeIdentifierConverter>();
            requiredConverterMock
                .SetupGet(converter => converter.TypeIdentifierFormat)
                .Returns(typeIdentifierFormat);

            converters.Add(requiredConverterMock.Object);

            Assert.Single(converters, converter => converter.TypeIdentifierFormat == typeIdentifierFormat);

            _ = new EventStreamEventTypeIdentifierConverterProvider(configurationMock.Object, converters);
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_InvalidOperationException_When_Creating_WithNotNullParameters_And_ConfigurationReturnsNullForgettablePayloadTypeIdentifierFormat_And_ThereAreMultipleConvertersProvided(
            Mock<IEventSourcingTypeConversionConfiguration> configurationMock,
            List<IEventStreamEventTypeIdentifierConverter> converters)
        {
            configurationMock
                .SetupGet(configuration => configuration.EventTypeIdentifierFormat)
                .Returns(null as EventStreamEventTypeIdentifierFormat);
            
            Assert.Throws<InvalidOperationException>(() =>
                new EventStreamEventTypeIdentifierConverterProvider(configurationMock.Object, converters));
        }
        
        [Theory]
        [AutoMoqData]
        public void NotThrow_When_Creating_WithNotNullParameters_And_ConfigurationReturnsNullForgettablePayloadTypeIdentifierFormat_And_ThereIsASingleConverterProvided(
            Mock<IEventSourcingTypeConversionConfiguration> configurationMock,
            IEventStreamEventTypeIdentifierConverter converter)
        {
            configurationMock
                .SetupGet(configuration => configuration.EventTypeIdentifierFormat)
                .Returns(null as EventStreamEventTypeIdentifierFormat);

            _ = new EventStreamEventTypeIdentifierConverterProvider(configurationMock.Object, new[] { converter });
        }
        
        [Theory]
        [AutoMoqData]
        public void NotThrow_When_GettingForgettablePayloadTypeIdentifierConverter_And_Created_WithNotNullParameters_And_ConfigurationReturnsNotNullForgettablePayloadTypeIdentifierFormat_And_ThereIsAConverterProvidedThatHandlesThisFormat(
            Mock<IEventSourcingTypeConversionConfiguration> configurationMock,
            List<IEventStreamEventTypeIdentifierConverter> converters,
            EventStreamEventTypeIdentifierFormat typeIdentifierFormat)
        {
            configurationMock
                .SetupGet(configuration => configuration.EventTypeIdentifierFormat)
                .Returns(typeIdentifierFormat);

            var requiredConverterMock = new Mock<IEventStreamEventTypeIdentifierConverter>();
            requiredConverterMock
                .SetupGet(converter => converter.TypeIdentifierFormat)
                .Returns(typeIdentifierFormat);

            converters.Add(requiredConverterMock.Object);

            Assert.Single(converters, converter => converter.TypeIdentifierFormat == typeIdentifierFormat);

            var provider = new EventStreamEventTypeIdentifierConverterProvider(configurationMock.Object, converters);
            
            Assert.Equal(requiredConverterMock.Object, provider.GetEventTypeIdentifierConverter());
        }
        
        [Theory]
        [AutoMoqData]
        public void NotThrow_When_GettingForgettablePayloadTypeIdentifierConverter_And_Created_WithNotNullParameters_And_ConfigurationReturnsNullForgettablePayloadTypeIdentifierFormat_And_ThereIsASingleConverterProvided(
            Mock<IEventSourcingTypeConversionConfiguration> configurationMock,
            IEventStreamEventTypeIdentifierConverter converter)
        {
            configurationMock
                .SetupGet(configuration => configuration.EventTypeIdentifierFormat)
                .Returns(null as EventStreamEventTypeIdentifierFormat);

            var provider = new EventStreamEventTypeIdentifierConverterProvider(configurationMock.Object, new[] { converter });
            
            Assert.Equal(converter, provider.GetEventTypeIdentifierConverter());
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CallingGetConverterWithNullParameter_And_Created_WithNotNullParameters_And_ConfigurationReturnsNotNullForgettablePayloadTypeIdentifierFormat_And_ThereIsAConverterProvidedThatHandlesThisFormat(
            Mock<IEventSourcingTypeConversionConfiguration> configurationMock,
            List<IEventStreamEventTypeIdentifierConverter> converters,
            EventStreamEventTypeIdentifierFormat typeIdentifierFormat)
        {
            configurationMock
                .SetupGet(configuration => configuration.EventTypeIdentifierFormat)
                .Returns(typeIdentifierFormat);

            var requiredConverterMock = new Mock<IEventStreamEventTypeIdentifierConverter>();
            requiredConverterMock
                .SetupGet(converter => converter.TypeIdentifierFormat)
                .Returns(typeIdentifierFormat);

            converters.Add(requiredConverterMock.Object);

            Assert.Single(converters, converter => converter.TypeIdentifierFormat == typeIdentifierFormat);

            var provider = new EventStreamEventTypeIdentifierConverterProvider(configurationMock.Object, converters);
            
            Assert.Throws<ArgumentNullException>(() => provider.GetConverter(null));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CallingGetConverterWithNullParameter_And_Created_WithNotNullParameters_And_ConfigurationReturnsNullForgettablePayloadTypeIdentifierFormat_And_ThereIsASingleConverterProvided(
            Mock<IEventSourcingTypeConversionConfiguration> configurationMock,
            IEventStreamEventTypeIdentifierConverter converter)
        {
            configurationMock
                .SetupGet(configuration => configuration.EventTypeIdentifierFormat)
                .Returns(null as EventStreamEventTypeIdentifierFormat);

            var provider = new EventStreamEventTypeIdentifierConverterProvider(configurationMock.Object, new[] { converter });
            
            Assert.Throws<ArgumentNullException>(() => provider.GetConverter(null));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_InvalidOperationException_When_CallingGetConverterWithTypeIdentifierFormatNotHandledByAnyProvidedConvertersDuringCreation_And_Created_WithNotNullParameters_And_ConfigurationReturnsNotNullForgettablePayloadTypeIdentifierFormat_And_ThereIsAConverterProvidedThatHandlesThisFormat(
            Mock<IEventSourcingTypeConversionConfiguration> configurationMock,
            List<IEventStreamEventTypeIdentifierConverter> converters,
            EventStreamEventTypeIdentifierFormat typeIdentifierFormat,
            EventStreamEventTypeIdentifierFormat typeIdentifierFormatToGet)
        {
            configurationMock
                .SetupGet(configuration => configuration.EventTypeIdentifierFormat)
                .Returns(typeIdentifierFormat);

            var requiredConverterMock = new Mock<IEventStreamEventTypeIdentifierConverter>();
            requiredConverterMock
                .SetupGet(converter => converter.TypeIdentifierFormat)
                .Returns(typeIdentifierFormat);

            converters.Add(requiredConverterMock.Object);

            Assert.Single(converters, converter => converter.TypeIdentifierFormat == typeIdentifierFormat);

            var provider = new EventStreamEventTypeIdentifierConverterProvider(configurationMock.Object, converters);
            
            Assert.Throws<InvalidOperationException>(() => provider.GetConverter(typeIdentifierFormatToGet));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CallingGetConverterWithTypeIdentifierFormatNotHandledByAnyProvidedConvertersDuringCreation_And_Created_WithNotNullParameters_And_ConfigurationReturnsNullForgettablePayloadTypeIdentifierFormat_And_ThereIsASingleConverterProvided(
            Mock<IEventSourcingTypeConversionConfiguration> configurationMock,
            IEventStreamEventTypeIdentifierConverter converter,
            EventStreamEventTypeIdentifierFormat typeIdentifierFormatToGet)
        {
            configurationMock
                .SetupGet(configuration => configuration.EventTypeIdentifierFormat)
                .Returns(null as EventStreamEventTypeIdentifierFormat);

            var provider = new EventStreamEventTypeIdentifierConverterProvider(configurationMock.Object, new[] { converter });
            
            Assert.Throws<InvalidOperationException>(() => provider.GetConverter(typeIdentifierFormatToGet));
        }
        
        [Theory]
        [AutoMoqWithInlineData(0)]
        [AutoMoqWithInlineData(1)]
        [AutoMoqWithInlineData(2)]
        [AutoMoqWithInlineData(3)]
        public void ReturnConverter_When_CallingGetConverterWithTypeIdentifierFormatHandledByOneOfProvidedConvertersDuringCreation_And_Created_WithNotNullParameters_And_ConfigurationReturnsNotNullForgettablePayloadTypeIdentifierFormat_And_ThereIsAConverterProvidedThatHandlesThisFormat(
            int indexOfConverterToGet,
            Mock<IEventSourcingTypeConversionConfiguration> configurationMock,
            List<IEventStreamEventTypeIdentifierConverter> converters,
            EventStreamEventTypeIdentifierFormat typeIdentifierFormat)
        {
            configurationMock
                .SetupGet(configuration => configuration.EventTypeIdentifierFormat)
                .Returns(typeIdentifierFormat);

            var requiredConverterMock = new Mock<IEventStreamEventTypeIdentifierConverter>();
            requiredConverterMock
                .SetupGet(converter => converter.TypeIdentifierFormat)
                .Returns(typeIdentifierFormat);

            converters.Add(requiredConverterMock.Object);

            Assert.Single(converters, converter => converter.TypeIdentifierFormat == typeIdentifierFormat);

            var provider = new EventStreamEventTypeIdentifierConverterProvider(configurationMock.Object, converters);

            var expectedConverter = converters[indexOfConverterToGet];
            Assert.Equal(expectedConverter, provider.GetConverter(expectedConverter.TypeIdentifierFormat));
        }
        
        [Theory]
        [AutoMoqData]
        public void ReturnConverter_When_CallingGetConverterWithTypeIdentifierFormatHandledByOneOfProvidedConvertersDuringCreation_And_Created_WithNotNullParameters_And_ConfigurationReturnsNullForgettablePayloadTypeIdentifierFormat_And_ThereIsASingleConverterProvided(
            Mock<IEventSourcingTypeConversionConfiguration> configurationMock,
            IEventStreamEventTypeIdentifierConverter converter)
        {
            configurationMock
                .SetupGet(configuration => configuration.EventTypeIdentifierFormat)
                .Returns(null as EventStreamEventTypeIdentifierFormat);

            var provider = new EventStreamEventTypeIdentifierConverterProvider(configurationMock.Object, new[] { converter });
            
            Assert.Equal(converter, provider.GetConverter(converter.TypeIdentifierFormat));
        }
    }
}