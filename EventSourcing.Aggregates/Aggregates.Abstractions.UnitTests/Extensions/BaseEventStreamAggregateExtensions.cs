using System;
using System.Reflection;
using EventSourcing.Abstractions;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.Aggregates.Abstractions;
using Xunit;

namespace Aggregates.Abstractions.UnitTests.Extensions
{
    public static class BaseEventStreamAggregateExtensions
    {
        private const BindingFlags NonPublicPropertyGetterBindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetProperty;
        private const BindingFlags NonPublicMethodBindingFlags = BindingFlags.Instance | BindingFlags.NonPublic; 
        
        public static AppendableEventStream GetAppendableEventStream<T>(this T aggregate)
            where T : BaseEventStreamAggregate
        {
            var appendableEventStreamGetter = typeof(T).GetMethod($"get_{nameof(AppendableEventStream)}", NonPublicPropertyGetterBindingFlags);
            Assert.NotNull(appendableEventStreamGetter);
            return (AppendableEventStream) appendableEventStreamGetter.Invoke(aggregate, new object[0]);
        }
        
        public static PublishableEventStream GetPublishableEventStream<T>(this T aggregate)
            where T : BaseEventStreamAggregate
        {
            var eventStreamAggregate = aggregate as IEventStreamAggregate;
            return eventStreamAggregate.PublishableEventStream;
        }
        
        public static void ReplayEventStream<T>(this T aggregate, EventStream eventStream)
            where T : BaseEventStreamAggregate
        {
            var eventStreamAggregate = aggregate as IEventStreamAggregate;
            eventStreamAggregate.ReplayEventStream(eventStream);
        }
        
        public static void AppendEvent<T>(this T aggregate, object @event)
            where T : BaseEventStreamAggregate
        {
            var appendEventMethodInfo = typeof(T).GetMethod(nameof(AppendEvent), NonPublicMethodBindingFlags);
            Assert.NotNull(appendEventMethodInfo);
            try
            {
                appendEventMethodInfo.Invoke(aggregate, new object[] {@event});
            }
            catch (TargetInvocationException e)
            {
                if (e.InnerException != null)
                {
                    throw e.InnerException;
                }

                throw;
            }
        }
        
        public static void ReplaySingleEvent<T>(this T aggregate, EventStreamEventWithMetadata eventWithMetadata)
            where T : BaseEventStreamAggregate
        {
            var replaySingleEventMethodInfo = typeof(T).GetMethod(nameof(ReplaySingleEvent), NonPublicMethodBindingFlags);
            Assert.NotNull(replaySingleEventMethodInfo);
            try
            {
                replaySingleEventMethodInfo.Invoke(aggregate, new object[] {eventWithMetadata});
            }
            catch (TargetInvocationException e)
            {
                if (e.InnerException != null)
                {
                    throw e.InnerException;
                }

                throw;
            }
        }
    }
}