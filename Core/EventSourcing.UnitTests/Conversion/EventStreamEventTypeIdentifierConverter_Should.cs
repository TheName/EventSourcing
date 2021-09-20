using System;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.Conversion;
using EventSourcing.UnitTests.ReferencedAssembly;
using TestHelpers.Attributes;
using Xunit;

namespace EventSourcing.UnitTests.Conversion
{
    public class EventStreamEventTypeIdentifierConverter_Should
    {
        [Theory]
        [AutoMoqData]
        internal void Throw_When_TryingToGetIdentifierForTypeOfAbstractClass(EventStreamEventTypeIdentifierConverter typeIdentifierConverter)
        {
            Assert.Throws<Exception>(() => typeIdentifierConverter.ToTypeIdentifier(typeof(AbstractClass)));
        }
        
        [Theory]
        [AutoMoqData]
        internal void Throw_When_TryingToGetIdentifierForTypeOfAnInterface(EventStreamEventTypeIdentifierConverter typeIdentifierConverter)
        {
            Assert.Throws<Exception>(() => typeIdentifierConverter.ToTypeIdentifier(typeof(IInterface)));
        }
        
        [Theory]
        [AutoMoqWithInlineData(typeof(NamespaceA.DuplicateClassName))]
        [AutoMoqWithInlineData(typeof(NamespaceB.DuplicateClassName))]
        internal void Throw_When_TryingToGetIdentifierForTypeWithDuplicatedNameAcrossNamespaces(
            Type type,
            EventStreamEventTypeIdentifierConverter typeIdentifierConverter)
        {
            Assert.Throws<Exception>(() => typeIdentifierConverter.ToTypeIdentifier(type));
        }
        
        [Theory]
        [AutoMoqData]
        internal void ReturnTypeIdentifier_When_TryingToGetIdentifierForValidType(EventStreamEventTypeIdentifierConverter typeIdentifierConverter)
        {
            var result = typeIdentifierConverter.ToTypeIdentifier(typeof(RandomClass));
            
            Assert.Equal(nameof(RandomClass), result);
        }
        
        [Theory]
        [AutoMoqData]
        internal void ReturnTypeIdentifier_When_TryingToGetIdentifierForValidTypeFromReferencedAssembly(EventStreamEventTypeIdentifierConverter typeIdentifierConverter)
        {
            var result = typeIdentifierConverter.ToTypeIdentifier(typeof(SampleClassFromReferencedAssembly));
            
            Assert.Equal(nameof(SampleClassFromReferencedAssembly), result);
        }
        
        [Theory]
        [AutoMoqData]
        internal void Throw_When_TryingToGetTypeForRandomIdentifierForWhichNoClassIsRegistered(
            EventStreamEventTypeIdentifier typeIdentifier,
            EventStreamEventTypeIdentifierConverter typeIdentifierConverter)
        {
            Assert.Throws<Exception>(() => typeIdentifierConverter.FromTypeIdentifier(typeIdentifier));
        }
        
        [Theory]
        [AutoMoqData]
        internal void Throw_When_TryingToGetTypeForIdentifierOfAnAbstractClass(EventStreamEventTypeIdentifierConverter typeIdentifierConverter)
        {
            Assert.Throws<Exception>(() => typeIdentifierConverter.FromTypeIdentifier(nameof(AbstractClass)));
        }
        
        [Theory]
        [AutoMoqData]
        internal void Throw_When_TryingToGetTypeForIdentifierOfAnInterface(EventStreamEventTypeIdentifierConverter typeIdentifierConverter)
        {
            Assert.Throws<Exception>(() => typeIdentifierConverter.FromTypeIdentifier(nameof(IInterface)));
        }
        
        [Theory]
        [AutoMoqWithInlineData(nameof(NamespaceA.DuplicateClassName))]
        [AutoMoqWithInlineData(nameof(NamespaceB.DuplicateClassName))]
        internal void Throw_When_TryingToGetTypeForIdentifierWithDuplicatedNameAcrossNamespaces(
            string identifier,
            EventStreamEventTypeIdentifierConverter typeIdentifierConverter)
        {
            Assert.Throws<Exception>(() => typeIdentifierConverter.FromTypeIdentifier(identifier));
        }
        
        [Theory]
        [AutoMoqData]
        internal void ReturnType_When_TryingToGetTypeForIdentifierRepresentingAValidType(EventStreamEventTypeIdentifierConverter typeIdentifierConverter)
        {
            var result = typeIdentifierConverter.FromTypeIdentifier(nameof(RandomClass));
            
            Assert.Equal(typeof(RandomClass), result);
        }
        
        [Theory]
        [AutoMoqData]
        internal void ReturnType_When_TryingToGetTypeForIdentifierRepresentingAValidTypeFromReferencedAssembly(EventStreamEventTypeIdentifierConverter typeIdentifierConverter)
        {
            var result = typeIdentifierConverter.FromTypeIdentifier(nameof(SampleClassFromReferencedAssembly));
            
            Assert.Equal(typeof(SampleClassFromReferencedAssembly), result);
        }
        
        private abstract class AbstractClass
        {
        }
        
        private interface IInterface
        {
        }
        
        private class RandomClass
        {
        }
    }
}

namespace NamespaceA
{
    internal class DuplicateClassName
    {
    }
}

namespace NamespaceB
{
    internal class DuplicateClassName
    {
    }
}