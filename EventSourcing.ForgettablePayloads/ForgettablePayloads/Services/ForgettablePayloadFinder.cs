using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EventSourcing.ForgettablePayloads.Abstractions;
using EventSourcing.ForgettablePayloads.Abstractions.Services;

namespace EventSourcing.ForgettablePayloads.Services
{
    internal class ForgettablePayloadFinder : IForgettablePayloadFinder
    {
        private static readonly Dictionary<Type, IReadOnlyCollection<Func<object, IEnumerable<ForgettablePayload>>>> ForgettablePayloadFetchingFunctionsByType =
                new Dictionary<Type, IReadOnlyCollection<Func<object, IEnumerable<ForgettablePayload>>>>();

        public IReadOnlyCollection<ForgettablePayload> Find(object @event)
        {
            if (@event == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }
            
            return GetForgettablePayloadRetrieversForType(@event.GetType())
                .SelectMany(func => func(@event))
                .Where(payload => payload != null)
                .ToList();
        }

        private static IEnumerable<Func<object, IEnumerable<ForgettablePayload>>> GetForgettablePayloadRetrieversForType(Type type)
        {
            if (ForgettablePayloadFetchingFunctionsByType.TryGetValue(type, out var retrievers))
            {
                return retrievers;
            }

            var result = FindForgettablePayloadRetrieversForType(type);
            ForgettablePayloadFetchingFunctionsByType.Add(type, result);

            return result;
        }

        private static IReadOnlyList<Func<object, IEnumerable<ForgettablePayload>>> FindForgettablePayloadRetrieversForType(Type type)
        {
            var result = FindForgettablePayloadRetrieversFromPropertiesAndFieldsForType(type);
            if (!typeof(ForgettablePayload).IsAssignableFrom(type))
            {
                if (typeof(IEnumerable<ForgettablePayload>).IsAssignableFrom(type))
                {
                    result.Add(o => (IEnumerable<ForgettablePayload>)o);
                    return result;
                }

                var typeAsEnumerableGeneric = type.GetInterfaces().FirstOrDefault(t => typeof(IEnumerable<object>).IsAssignableFrom(t));
                if (typeAsEnumerableGeneric != null)
                {
                    var enumerableType = typeAsEnumerableGeneric.GetGenericArguments().Single();
                    var retrieversForEnumerableType = GetForgettablePayloadRetrieversForType(enumerableType);

                    result.AddRange(retrieversForEnumerableType
                        .Select(func =>
                            (Func<object, IEnumerable<ForgettablePayload>>)(o =>
                                ((IEnumerable<object>)o).SelectMany(func))));
                }
                
                return result;
            }

            if (result.Any())
            {
                throw new InvalidOperationException(
                    $"Type {type} has nested public properties with public getters or public fields of {typeof(ForgettablePayload)} type. This is not supported.");
            }

            return new List<Func<object, IEnumerable<ForgettablePayload>>> { o => new[] { (ForgettablePayload)o } };
        }

        private static List<Func<object, IEnumerable<ForgettablePayload>>> FindForgettablePayloadRetrieversFromPropertiesAndFieldsForType(Type type)
        {
            var result = new List<Func<object, IEnumerable<ForgettablePayload>>>();
            var propertyGetMethodInfos = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(info => info.CanRead && info.PropertyType != type)
                .Select(info => info.GetGetMethod(false))
                .Where(info => info != null && info.GetParameters().Length == 0);
            
            foreach (var getMethodInfo in propertyGetMethodInfos)
            {
                var payloadRetrieversForProperty = GetForgettablePayloadRetrieversForType(getMethodInfo.ReturnType);
                result.AddRange(payloadRetrieversForProperty.Select(func => new Func<object, IEnumerable<ForgettablePayload>>(o => func(getMethodInfo.Invoke(o, Array.Empty<object>())))));
            }

            var fieldInfoToCheck = type.GetFields(BindingFlags.Instance | BindingFlags.Public)
                .Where(info => info.FieldType != type);
            
            foreach (var fieldInfo in fieldInfoToCheck)
            {
                var payloadRetrieversForField = GetForgettablePayloadRetrieversForType(fieldInfo.FieldType);
                result.AddRange(payloadRetrieversForField.Select(func => new Func<object, IEnumerable<ForgettablePayload>>(o => func(fieldInfo.GetValue(o)))));
            }

            return result;
        }
    }
}