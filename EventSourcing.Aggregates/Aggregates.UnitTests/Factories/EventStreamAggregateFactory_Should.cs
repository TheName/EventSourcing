using System;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.Aggregates.Abstractions;
using EventSourcing.Aggregates.Factories;
using TestHelpers.Attributes;
using Xunit;

namespace Aggregates.UnitTests.Factories
{
    public class EventStreamAggregateFactory_Should
    {
        [Theory]
        [AutoMoqData]
        internal void Throw_ArgumentNullException_When_TryingToCreate_And_PassingNullAsAggregateType(EventStreamAggregateFactory factory)
        {
            Assert.Throws<ArgumentNullException>(() => factory.Create(null));
        }
        
        [Theory]
        [AutoMoqWithInlineData(typeof(BaseEventStreamAggregate))]
        internal void Throw_ArgumentNullException_When_TryingToCreate_And_PassingAbstractType(
            Type aggregateType,
            EventStreamAggregateFactory factory)
        {
            Assert.Throws<ArgumentException>(() => factory.Create(aggregateType));
        }
        
        [Theory]
        [AutoMoqWithInlineData(typeof(IEventStreamAggregate))]
        internal void Throw_ArgumentNullException_When_TryingToCreate_And_PassingInterfaceType(
            Type aggregateType,
            EventStreamAggregateFactory factory)
        {
            Assert.Throws<ArgumentException>(() => factory.Create(aggregateType));
        }

        [Theory]
        [AutoMoqWithInlineData(typeof(ClassWithoutParameterlessConstructor))]
        internal void Throw_MissingMethodException_When_TryingToCreateObjectWithoutParameterlessConstructor(Type aggregateType, EventStreamAggregateFactory factory)
        {
            Assert.Throws<MissingMethodException>(() => factory.Create(aggregateType));
        }
        
        [Theory]
        [AutoMoqWithInlineData(typeof(ClassWithPublicParameterlessConstructor))]
        [AutoMoqWithInlineData(typeof(ClassWithPrivateParameterlessConstructor))]
        internal void CreateObject_When_TryingToCreateObjectWithParameterlessConstructor(Type aggregateType, EventStreamAggregateFactory factory)
        {
            var result = factory.Create(aggregateType);

            Assert.NotNull(result);
        }
        
        [Theory]
        [AutoMoqWithInlineData(typeof(ClassWithConstructorAcceptingStreamIdOnly))]
        internal void CreateObject_When_TryingToCreateObjectWithConstructorAcceptingStreamIdOnly(Type aggregateType, EventStreamAggregateFactory factory)
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
        
        private class ClassWithConstructorAcceptingStreamIdOnly
        {
            private readonly EventStreamId _streamId;

            public ClassWithConstructorAcceptingStreamIdOnly(EventStreamId streamId)
            {
                _streamId = streamId;
            }
        }
    }
}