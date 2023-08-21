using System;
using System.Collections.Generic;
using System.Linq;
using EventSourcing.ForgettablePayloads.Configurations;
using EventSourcing.ForgettablePayloads.Conversion;
using EventSourcing.ForgettablePayloads.ValueObjects;
using Moq;
using TestHelpers.Attributes;
using Xunit;

namespace ForgettablePayloads.UnitTests.Conversion
{
    public class ForgettablePayloadTypeIdentifierConverterProvider_Should
    {
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_Creating_WithNullConfiguration(
            IEnumerable<IForgettablePayloadTypeIdentifierConverter> converters)
        {
            Assert.Throws<ArgumentNullException>(() =>
                new ForgettablePayloadTypeIdentifierConverterProvider(null, converters));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_Creating_WithNullConverters(
            IForgettablePayloadTypeConversionConfiguration configuration)
        {
            Assert.Throws<ArgumentNullException>(() =>
                new ForgettablePayloadTypeIdentifierConverterProvider(configuration, null));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentException_When_Creating_WithNotNullParameters_And_ThereAreMultipleConvertersReturningSameTypeIdentifierFormat(
            IForgettablePayloadTypeConversionConfiguration configuration,
            IEnumerable<IForgettablePayloadTypeIdentifierConverter> converters,
            ForgettablePayloadTypeIdentifierFormat typeIdentifierFormat)
        {
            var converterMocksReturningSameTypeIdentifierFormat = Enumerable.Range(0, 3)
                .Select(i => new Mock<IForgettablePayloadTypeIdentifierConverter>())
                .ToList();

            converterMocksReturningSameTypeIdentifierFormat
                .ForEach(mock => mock
                    .SetupGet(converter => converter.TypeIdentifierFormat)
                    .Returns(typeIdentifierFormat));

            var allConverters = new List<IForgettablePayloadTypeIdentifierConverter>(converters);
            allConverters.AddRange(converterMocksReturningSameTypeIdentifierFormat.Select(mock => mock.Object));

            Assert.Throws<ArgumentException>(() =>
                new ForgettablePayloadTypeIdentifierConverterProvider(configuration, allConverters));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_InvalidOperationException_When_Creating_WithNotNullParameters_And_ConfigurationReturnsNotNullForgettablePayloadTypeIdentifierFormat_And_ThereIsNoConverterProvidedThatHandlesThisFormat(
            Mock<IForgettablePayloadTypeConversionConfiguration> configurationMock,
            List<IForgettablePayloadTypeIdentifierConverter> converters,
            ForgettablePayloadTypeIdentifierFormat typeIdentifierFormat)
        {
            configurationMock
                .SetupGet(configuration => configuration.ForgettablePayloadTypeIdentifierFormat)
                .Returns(typeIdentifierFormat);
            
            Assert.True(converters.All(converter => converter.TypeIdentifierFormat != typeIdentifierFormat));
            
            Assert.Throws<InvalidOperationException>(() =>
                new ForgettablePayloadTypeIdentifierConverterProvider(configurationMock.Object, converters));
        }
        
        [Theory]
        [AutoMoqData]
        public void NotThrow_When_Creating_WithNotNullParameters_And_ConfigurationReturnsNotNullForgettablePayloadTypeIdentifierFormat_And_ThereIsAConverterProvidedThatHandlesThisFormat(
            Mock<IForgettablePayloadTypeConversionConfiguration> configurationMock,
            List<IForgettablePayloadTypeIdentifierConverter> converters,
            ForgettablePayloadTypeIdentifierFormat typeIdentifierFormat)
        {
            configurationMock
                .SetupGet(configuration => configuration.ForgettablePayloadTypeIdentifierFormat)
                .Returns(typeIdentifierFormat);

            var requiredConverterMock = new Mock<IForgettablePayloadTypeIdentifierConverter>();
            requiredConverterMock
                .SetupGet(converter => converter.TypeIdentifierFormat)
                .Returns(typeIdentifierFormat);

            converters.Add(requiredConverterMock.Object);

            Assert.Single(converters, converter => converter.TypeIdentifierFormat == typeIdentifierFormat);

            _ = new ForgettablePayloadTypeIdentifierConverterProvider(configurationMock.Object, converters);
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_InvalidOperationException_When_Creating_WithNotNullParameters_And_ConfigurationReturnsNullForgettablePayloadTypeIdentifierFormat_And_ThereAreMultipleConvertersProvided(
            Mock<IForgettablePayloadTypeConversionConfiguration> configurationMock,
            List<IForgettablePayloadTypeIdentifierConverter> converters)
        {
            configurationMock
                .SetupGet(configuration => configuration.ForgettablePayloadTypeIdentifierFormat)
                .Returns(null as ForgettablePayloadTypeIdentifierFormat);
            
            Assert.Throws<InvalidOperationException>(() =>
                new ForgettablePayloadTypeIdentifierConverterProvider(configurationMock.Object, converters));
        }
        
        [Theory]
        [AutoMoqData]
        public void NotThrow_When_Creating_WithNotNullParameters_And_ConfigurationReturnsNullForgettablePayloadTypeIdentifierFormat_And_ThereIsASingleConverterProvided(
            Mock<IForgettablePayloadTypeConversionConfiguration> configurationMock,
            IForgettablePayloadTypeIdentifierConverter converter)
        {
            configurationMock
                .SetupGet(configuration => configuration.ForgettablePayloadTypeIdentifierFormat)
                .Returns(null as ForgettablePayloadTypeIdentifierFormat);

            _ = new ForgettablePayloadTypeIdentifierConverterProvider(configurationMock.Object, new[] { converter });
        }
        
        [Theory]
        [AutoMoqData]
        public void NotThrow_When_GettingForgettablePayloadTypeIdentifierConverter_And_Created_WithNotNullParameters_And_ConfigurationReturnsNotNullForgettablePayloadTypeIdentifierFormat_And_ThereIsAConverterProvidedThatHandlesThisFormat(
            Mock<IForgettablePayloadTypeConversionConfiguration> configurationMock,
            List<IForgettablePayloadTypeIdentifierConverter> converters,
            ForgettablePayloadTypeIdentifierFormat typeIdentifierFormat)
        {
            configurationMock
                .SetupGet(configuration => configuration.ForgettablePayloadTypeIdentifierFormat)
                .Returns(typeIdentifierFormat);

            var requiredConverterMock = new Mock<IForgettablePayloadTypeIdentifierConverter>();
            requiredConverterMock
                .SetupGet(converter => converter.TypeIdentifierFormat)
                .Returns(typeIdentifierFormat);

            converters.Add(requiredConverterMock.Object);

            Assert.Single(converters, converter => converter.TypeIdentifierFormat == typeIdentifierFormat);

            var provider = new ForgettablePayloadTypeIdentifierConverterProvider(configurationMock.Object, converters);
            
            Assert.Equal(requiredConverterMock.Object, provider.GetForgettablePayloadTypeIdentifierConverter());
        }
        
        [Theory]
        [AutoMoqData]
        public void NotThrow_When_GettingForgettablePayloadTypeIdentifierConverter_And_Created_WithNotNullParameters_And_ConfigurationReturnsNullForgettablePayloadTypeIdentifierFormat_And_ThereIsASingleConverterProvided(
            Mock<IForgettablePayloadTypeConversionConfiguration> configurationMock,
            IForgettablePayloadTypeIdentifierConverter converter)
        {
            configurationMock
                .SetupGet(configuration => configuration.ForgettablePayloadTypeIdentifierFormat)
                .Returns(null as ForgettablePayloadTypeIdentifierFormat);

            var provider = new ForgettablePayloadTypeIdentifierConverterProvider(configurationMock.Object, new[] { converter });
            
            Assert.Equal(converter, provider.GetForgettablePayloadTypeIdentifierConverter());
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CallingGetConverterWithNullParameter_And_Created_WithNotNullParameters_And_ConfigurationReturnsNotNullForgettablePayloadTypeIdentifierFormat_And_ThereIsAConverterProvidedThatHandlesThisFormat(
            Mock<IForgettablePayloadTypeConversionConfiguration> configurationMock,
            List<IForgettablePayloadTypeIdentifierConverter> converters,
            ForgettablePayloadTypeIdentifierFormat typeIdentifierFormat)
        {
            configurationMock
                .SetupGet(configuration => configuration.ForgettablePayloadTypeIdentifierFormat)
                .Returns(typeIdentifierFormat);

            var requiredConverterMock = new Mock<IForgettablePayloadTypeIdentifierConverter>();
            requiredConverterMock
                .SetupGet(converter => converter.TypeIdentifierFormat)
                .Returns(typeIdentifierFormat);

            converters.Add(requiredConverterMock.Object);

            Assert.Single(converters, converter => converter.TypeIdentifierFormat == typeIdentifierFormat);

            var provider = new ForgettablePayloadTypeIdentifierConverterProvider(configurationMock.Object, converters);
            
            Assert.Throws<ArgumentNullException>(() => provider.GetConverter(null));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CallingGetConverterWithNullParameter_And_Created_WithNotNullParameters_And_ConfigurationReturnsNullForgettablePayloadTypeIdentifierFormat_And_ThereIsASingleConverterProvided(
            Mock<IForgettablePayloadTypeConversionConfiguration> configurationMock,
            IForgettablePayloadTypeIdentifierConverter converter)
        {
            configurationMock
                .SetupGet(configuration => configuration.ForgettablePayloadTypeIdentifierFormat)
                .Returns(null as ForgettablePayloadTypeIdentifierFormat);

            var provider = new ForgettablePayloadTypeIdentifierConverterProvider(configurationMock.Object, new[] { converter });
            
            Assert.Throws<ArgumentNullException>(() => provider.GetConverter(null));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_InvalidOperationException_When_CallingGetConverterWithTypeIdentifierFormatNotHandledByAnyProvidedConvertersDuringCreation_And_Created_WithNotNullParameters_And_ConfigurationReturnsNotNullForgettablePayloadTypeIdentifierFormat_And_ThereIsAConverterProvidedThatHandlesThisFormat(
            Mock<IForgettablePayloadTypeConversionConfiguration> configurationMock,
            List<IForgettablePayloadTypeIdentifierConverter> converters,
            ForgettablePayloadTypeIdentifierFormat forgettablePayloadTypeIdentifierFormat,
            ForgettablePayloadTypeIdentifierFormat typeIdentifierFormatToGet)
        {
            configurationMock
                .SetupGet(configuration => configuration.ForgettablePayloadTypeIdentifierFormat)
                .Returns(forgettablePayloadTypeIdentifierFormat);

            var requiredConverterMock = new Mock<IForgettablePayloadTypeIdentifierConverter>();
            requiredConverterMock
                .SetupGet(converter => converter.TypeIdentifierFormat)
                .Returns(forgettablePayloadTypeIdentifierFormat);

            converters.Add(requiredConverterMock.Object);

            Assert.Single(converters, converter => converter.TypeIdentifierFormat == forgettablePayloadTypeIdentifierFormat);

            var provider = new ForgettablePayloadTypeIdentifierConverterProvider(configurationMock.Object, converters);
            
            Assert.Throws<InvalidOperationException>(() => provider.GetConverter(typeIdentifierFormatToGet));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CallingGetConverterWithTypeIdentifierFormatNotHandledByAnyProvidedConvertersDuringCreation_And_Created_WithNotNullParameters_And_ConfigurationReturnsNullForgettablePayloadTypeIdentifierFormat_And_ThereIsASingleConverterProvided(
            Mock<IForgettablePayloadTypeConversionConfiguration> configurationMock,
            IForgettablePayloadTypeIdentifierConverter converter,
            ForgettablePayloadTypeIdentifierFormat typeIdentifierFormatToGet)
        {
            configurationMock
                .SetupGet(configuration => configuration.ForgettablePayloadTypeIdentifierFormat)
                .Returns(null as ForgettablePayloadTypeIdentifierFormat);

            var provider = new ForgettablePayloadTypeIdentifierConverterProvider(configurationMock.Object, new[] { converter });
            
            Assert.Throws<InvalidOperationException>(() => provider.GetConverter(typeIdentifierFormatToGet));
        }
        
        [Theory]
        [AutoMoqWithInlineData(0)]
        [AutoMoqWithInlineData(1)]
        [AutoMoqWithInlineData(2)]
        [AutoMoqWithInlineData(3)]
        public void ReturnConverter_When_CallingGetConverterWithTypeIdentifierFormatHandledByOneOfProvidedConvertersDuringCreation_And_Created_WithNotNullParameters_And_ConfigurationReturnsNotNullForgettablePayloadTypeIdentifierFormat_And_ThereIsAConverterProvidedThatHandlesThisFormat(
            int indexOfConverterToGet,
            Mock<IForgettablePayloadTypeConversionConfiguration> configurationMock,
            List<IForgettablePayloadTypeIdentifierConverter> converters,
            ForgettablePayloadTypeIdentifierFormat forgettablePayloadTypeIdentifierFormat)
        {
            configurationMock
                .SetupGet(configuration => configuration.ForgettablePayloadTypeIdentifierFormat)
                .Returns(forgettablePayloadTypeIdentifierFormat);

            var requiredConverterMock = new Mock<IForgettablePayloadTypeIdentifierConverter>();
            requiredConverterMock
                .SetupGet(converter => converter.TypeIdentifierFormat)
                .Returns(forgettablePayloadTypeIdentifierFormat);

            converters.Add(requiredConverterMock.Object);

            Assert.Single(converters, converter => converter.TypeIdentifierFormat == forgettablePayloadTypeIdentifierFormat);

            var provider = new ForgettablePayloadTypeIdentifierConverterProvider(configurationMock.Object, converters);

            var expectedConverter = converters[indexOfConverterToGet];
            Assert.Equal(expectedConverter, provider.GetConverter(expectedConverter.TypeIdentifierFormat));
        }
        
        [Theory]
        [AutoMoqData]
        public void ReturnConverter_When_CallingGetConverterWithTypeIdentifierFormatHandledByOneOfProvidedConvertersDuringCreation_And_Created_WithNotNullParameters_And_ConfigurationReturnsNullForgettablePayloadTypeIdentifierFormat_And_ThereIsASingleConverterProvided(
            Mock<IForgettablePayloadTypeConversionConfiguration> configurationMock,
            IForgettablePayloadTypeIdentifierConverter converter)
        {
            configurationMock
                .SetupGet(configuration => configuration.ForgettablePayloadTypeIdentifierFormat)
                .Returns(null as ForgettablePayloadTypeIdentifierFormat);

            var provider = new ForgettablePayloadTypeIdentifierConverterProvider(configurationMock.Object, new[] { converter });
            
            Assert.Equal(converter, provider.GetConverter(converter.TypeIdentifierFormat));
        }
    }
}