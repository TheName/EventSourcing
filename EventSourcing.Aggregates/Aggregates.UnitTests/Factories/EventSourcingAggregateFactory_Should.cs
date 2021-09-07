using System;
using EventSourcing.Aggregates.Factories;
using TestHelpers.Attributes;
using Xunit;

namespace Aggregates.UnitTests.Factories
{
    public class EventSourcingAggregateFactory_Should
    {
        [Theory]
        [AutoMoqWithInlineData(typeof(ClassWithoutParameterlessConstructor))]
        internal void Throw_MissingMethodException_When_TryingToCreateObjectWithoutParameterlessConstructor(Type aggregateType, EventSourcingAggregateFactory factory)
        {
            Assert.Throws<MissingMethodException>(() => factory.Create(aggregateType));
        }
        
        [Theory]
        [AutoMoqWithInlineData(typeof(ClassWithPublicParameterlessConstructor))]
        [AutoMoqWithInlineData(typeof(ClassWithPrivateParameterlessConstructor))]
        internal void CreateObject_When_TryingToCreateObjectWithParameterlessConstructor(Type aggregateType, EventSourcingAggregateFactory factory)
        {
            var result = factory.Create(aggregateType);

            Assert.NotNull(result);
        }
        
        private class ClassWithPublicParameterlessConstructor
        {
        }
        
        private class ClassWithPrivateParameterlessConstructor
        {
            private ClassWithPrivateParameterlessConstructor()
            {
            }
        }
        
        private class ClassWithoutParameterlessConstructor
        {
            private readonly bool _someParam;

            public ClassWithoutParameterlessConstructor(bool someParam)
            {
                _someParam = someParam;
            }
        }
    }
}