using System;
using System.Reflection;

namespace Bus.RabbitMQ.UnitTests.Extensions
{
    public static class ObjectExtensions
    {
        public static T GetPrivateFieldValue<T>(this object instance, string fieldName)
        {
            var fieldInfo = instance.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);

            if (fieldInfo == null)
            {
                throw new InvalidOperationException($"Could not find field info for \"{fieldName}\" private field");
            }

            return (T) fieldInfo.GetValue(instance);
        }
    }
}