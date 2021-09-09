using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EventSourcing.Abstractions.ValueObjects;

namespace EventSourcing.Aggregates.Abstractions.Helpers
{
    internal static class EventStreamAggregateMethodInfoRepository
    {
        private static readonly Dictionary<Type, Dictionary<Type, List<MethodInfo>>> EventWithMetadataHandlers;
        private static readonly Dictionary<Type, Dictionary<Type, List<MethodInfo>>> EventHandlers;
        
        static EventStreamAggregateMethodInfoRepository()
        {
            var aggregateTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => !type.IsAbstract && !type.IsInterface && typeof(BaseEventStreamAggregate).IsAssignableFrom(type))
                .ToList();

            EventWithMetadataHandlers = aggregateTypes
                .ToDictionary(
                    aggregateType => aggregateType,
                    aggregateType => aggregateType
                        .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                        .Where(info =>
                            !info.IsSpecialName && !info.IsAbstract && info.GetParameters().Length == 2 &&
                            info.GetParameters()[1].ParameterType == typeof(EventStreamEventMetadata))
                        .GroupBy(info => info.GetParameters()[0].ParameterType)
                        .ToDictionary(infos => infos.Key, infos => infos.ToList()));
            
            EventHandlers = aggregateTypes
                .ToDictionary(
                    aggregateType => aggregateType,
                    aggregateType => aggregateType
                        .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                        .Where(info => !info.IsSpecialName && !info.IsAbstract && info.GetParameters().Length == 1)
                        .GroupBy(info => info.GetParameters()[0].ParameterType)
                        .ToDictionary(infos => infos.Key, infos => infos.ToList()));
        }
        
        public static bool TryGetMethodInfoForAggregateTypeAndEventType(Type aggregateType, Type eventType, out MethodInfo methodInfo)
        {
            if (!EventHandlers.TryGetValue(aggregateType, out var handlers))
            {
                throw new InvalidOperationException($"Did not find an aggregate of type {aggregateType}.");
            }

            if (!handlers.TryGetValue(eventType, out var methodInfos))
            {
                methodInfo = null;
                return false;
            }

            if (methodInfos.Count != 1)
            {
                throw new InvalidOperationException($"Found more than one methods handling type {eventType} in type {aggregateType}.");
            }

            methodInfo = methodInfos.Single();
            return true;
        }
        
        public static bool TryGetMethodInfoForAggregateTypeAndEventTypeAndEventMetadata(Type aggregateType, Type eventType, out MethodInfo methodInfo)
        {
            if (!EventWithMetadataHandlers.TryGetValue(aggregateType, out var handlers))
            {
                throw new InvalidOperationException($"Did not find an aggregate of type {aggregateType}.");
            }

            if (!handlers.TryGetValue(eventType, out var methodInfos))
            {
                methodInfo = null;
                return false;
            }

            if (methodInfos.Count != 1)
            {
                throw new InvalidOperationException($"Found more than one methods handling type {eventType} and {typeof(EventStreamEventMetadata)} in type {aggregateType}.");
            }

            methodInfo = methodInfos.Single();
            return true;
        }
    }
}