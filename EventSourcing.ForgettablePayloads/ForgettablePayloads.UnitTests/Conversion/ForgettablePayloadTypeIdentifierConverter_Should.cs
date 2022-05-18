using System;
using EventSourcing.ForgettablePayloads.Abstractions.ValueObjects;
using EventSourcing.ForgettablePayloads.Conversion;
using ForgettablePayloads.UnitTests.ReferencedAssembly;
using TestHelpers.Attributes;
using Xunit;

namespace ForgettablePayloads.UnitTests.Conversion
{
    public class ForgettablePayloadTypeIdentifierConverter_Should
    {
        [Theory]
        [AutoMoqData]
        internal void Throw_When_TryingToGetIdentifierForTypeOfAbstractClass(ForgettablePayloadTypeIdentifierConverter typeIdentifierConverter)
        {
            Assert.Throws<Exception>(() => typeIdentifierConverter.ToTypeIdentifier(typeof(AbstractClass)));
        }
        
        [Theory]
        [AutoMoqData]
        internal void Throw_When_TryingToGetIdentifierForTypeOfAnInterface(ForgettablePayloadTypeIdentifierConverter typeIdentifierConverter)
        {
            Assert.Throws<Exception>(() => typeIdentifierConverter.ToTypeIdentifier(typeof(IInterface)));
        }
        
        [Theory]
        [AutoMoqWithInlineData(typeof(NamespaceA.DuplicateClassName))]
        [AutoMoqWithInlineData(typeof(NamespaceB.DuplicateClassName))]
        internal void Throw_When_TryingToGetIdentifierForTypeWithDuplicatedNameAcrossNamespaces(
            Type type,
            ForgettablePayloadTypeIdentifierConverter typeIdentifierConverter)
        {
            Assert.Throws<Exception>(() => typeIdentifierConverter.ToTypeIdentifier(type));
        }
        
        [Theory]
        [AutoMoqData]
        internal void ReturnTypeIdentifier_When_TryingToGetIdentifierForValidType(ForgettablePayloadTypeIdentifierConverter typeIdentifierConverter)
        {
            var result = typeIdentifierConverter.ToTypeIdentifier(typeof(RandomClass));
            
            Assert.Equal(nameof(RandomClass), result);
        }
        
        [Theory]
        [AutoMoqData]
        internal void ReturnTypeIdentifier_When_TryingToGetIdentifierForValidTypeFromReferencedAssembly(ForgettablePayloadTypeIdentifierConverter typeIdentifierConverter)
        {
            var result = typeIdentifierConverter.ToTypeIdentifier(typeof(SampleClassFromReferencedAssembly));
            
            Assert.Equal(nameof(SampleClassFromReferencedAssembly), result);
        }
        
        [Theory]
        [AutoMoqData]
        internal void Throw_When_TryingToGetTypeForRandomIdentifierForWhichNoClassIsRegistered(
            ForgettablePayloadTypeIdentifier typeIdentifier,
            ForgettablePayloadTypeIdentifierConverter typeIdentifierConverter)
        {
            Assert.Throws<Exception>(() => typeIdentifierConverter.FromTypeIdentifier(typeIdentifier));
        }
        
        [Theory]
        [AutoMoqData]
        internal void Throw_When_TryingToGetTypeForIdentifierOfAnAbstractClass(ForgettablePayloadTypeIdentifierConverter typeIdentifierConverter)
        {
            Assert.Throws<Exception>(() => typeIdentifierConverter.FromTypeIdentifier(nameof(AbstractClass)));
        }
        
        [Theory]
        [AutoMoqData]
        internal void Throw_When_TryingToGetTypeForIdentifierOfAnInterface(ForgettablePayloadTypeIdentifierConverter typeIdentifierConverter)
        {
            Assert.Throws<Exception>(() => typeIdentifierConverter.FromTypeIdentifier(nameof(IInterface)));
        }
        
        [Theory]
        [AutoMoqWithInlineData(nameof(NamespaceA.DuplicateClassName))]
        [AutoMoqWithInlineData(nameof(NamespaceB.DuplicateClassName))]
        internal void Throw_When_TryingToGetTypeForIdentifierWithDuplicatedNameAcrossNamespaces(
            string identifier,
            ForgettablePayloadTypeIdentifierConverter typeIdentifierConverter)
        {
            Assert.Throws<Exception>(() => typeIdentifierConverter.FromTypeIdentifier(identifier));
        }
        
        [Theory]
        [AutoMoqData]
        internal void ReturnType_When_TryingToGetTypeForIdentifierRepresentingAValidType(ForgettablePayloadTypeIdentifierConverter typeIdentifierConverter)
        {
            var result = typeIdentifierConverter.FromTypeIdentifier(nameof(RandomClass));
            
            Assert.Equal(typeof(RandomClass), result);
        }
        
        [Theory]
        [AutoMoqData]
        internal void ReturnType_When_TryingToGetTypeForIdentifierRepresentingAValidTypeFromReferencedAssembly(ForgettablePayloadTypeIdentifierConverter typeIdentifierConverter)
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