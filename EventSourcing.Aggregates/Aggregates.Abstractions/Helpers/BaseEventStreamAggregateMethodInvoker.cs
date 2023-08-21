using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EventSourcing.ValueObjects;

namespace EventSourcing.Aggregates.Helpers
{
    internal static class BaseEventStreamAggregateMethodInvoker
    {
        private static readonly Dictionary<Type, BaseEventStreamAggregateMethodTracker> MethodTrackersByAggregateTypesDictionary;

        static BaseEventStreamAggregateMethodInvoker()
        {
            var nonAbstractAggregateTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => !type.IsAbstract && typeof(BaseEventStreamAggregate).IsAssignableFrom(type))
                .ToList();

            MethodTrackersByAggregateTypesDictionary = nonAbstractAggregateTypes.ToDictionary(
                type => type,
                type => new BaseEventStreamAggregateMethodTracker(type));
        }

        public static void Invoke(
            BaseEventStreamAggregate aggregate,
            EventStreamEventWithMetadata eventWithMetadata,
            bool shouldIgnoreMissingHandlers)
        {
            if (aggregate == null)
            {
                throw new ArgumentNullException(nameof(aggregate));
            }

            if (eventWithMetadata == null)
            {
                throw new ArgumentNullException(nameof(eventWithMetadata));
            }

            if (!MethodTrackersByAggregateTypesDictionary.TryGetValue(aggregate.GetType(), out var tracker))
            {
                throw new InvalidOperationException($"Did not find an aggregate of type {aggregate.GetType()}.");
            }

            var methodInfo = tracker.GetHandlerMethodInfoForEventType(eventWithMetadata.Event.GetType());
            if (methodInfo == null)
            {
                if (shouldIgnoreMissingHandlers)
                {
                    return;
                }

                throw new MissingMethodException(
                    $"Did not find a method that would handle event of type {eventWithMetadata.Event.GetType()} in aggregate of type {aggregate.GetType()}.");
            }

            var parameters = new List<object> {eventWithMetadata.Event};
            if (methodInfo.GetParameters().Length == 2)
            {
                parameters.Add(eventWithMetadata.EventMetadata);
            }

            methodInfo.Invoke(aggregate, parameters.ToArray());
        }

        private class BaseEventStreamAggregateMethodTracker
        {
            private readonly Type _aggregateType;

            private readonly Dictionary<Type, List<MethodInfo>> _eventHandlingMethodInfosByEventType;

            public BaseEventStreamAggregateMethodTracker(Type aggregateType)
            {
                _aggregateType = aggregateType;

                _eventHandlingMethodInfosByEventType = aggregateType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                    .Where(info => !info.IsSpecialName && !info.IsAbstract)
                    .Where(info => info.GetParameters().Length == 1 ||
                                   (info.GetParameters().Length == 2 && info.GetParameters()[1].ParameterType == typeof(EventStreamEventMetadata)))
                    .GroupBy(info => info.GetParameters()[0].ParameterType)
                    .ToDictionary(infos => infos.Key, infos => infos.ToList());
            }

            public MethodInfo GetHandlerMethodInfoForEventType(Type eventType)
            {
                if (!_eventHandlingMethodInfosByEventType.TryGetValue(eventType, out var methodInfos))
                {
                    return null;
                }

                if (methodInfos.Count != 1)
                {
                    throw new InvalidOperationException($"Found more than one methods handling type {eventType} in aggregate of type {_aggregateType}.");
                }

                return methodInfos.Single();

            }
        }
    }
}
