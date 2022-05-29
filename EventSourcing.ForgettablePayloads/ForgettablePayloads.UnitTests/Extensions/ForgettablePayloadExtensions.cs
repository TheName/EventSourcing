using System;
using System.Reflection;
using EventSourcing.ForgettablePayloads.Abstractions;
using EventSourcing.ForgettablePayloads.Abstractions.Conversion;
using EventSourcing.ForgettablePayloads.Abstractions.Services;

namespace ForgettablePayloads.UnitTests.Extensions
{
    internal static class ForgettablePayloadExtensions
    {
        public static IForgettablePayloadDescriptorLoader GetForgettablePayloadDescriptorLoader(
            this ForgettablePayload forgettablePayload)
        {
            return (IForgettablePayloadDescriptorLoader)GetPrivatePropertyInfo("PayloadDescriptorLoader")
                .GetValue(forgettablePayload);
        }
        
        public static IForgettablePayloadForgettingService GetForgettablePayloadForgettingService(
            this ForgettablePayload forgettablePayload)
        {
            return (IForgettablePayloadForgettingService)GetPrivatePropertyInfo("ForgettingService")
                .GetValue(forgettablePayload);
        }
        
        public static IForgettablePayloadClaimingService GetForgettablePayloadClaimingService(
            this ForgettablePayload forgettablePayload)
        {
            return (IForgettablePayloadClaimingService)GetPrivatePropertyInfo("ClaimingService")
                .GetValue(forgettablePayload);
        }
        
        public static IForgettablePayloadContentConverter GetForgettablePayloadContentConverter(
            this ForgettablePayload forgettablePayload)
        {
            return (IForgettablePayloadContentConverter)GetPrivatePropertyInfo("Converter")
                .GetValue(forgettablePayload);
        }

        private static PropertyInfo GetPrivatePropertyInfo(string propertyName)
        {
            var propertyInfo = typeof(ForgettablePayload).GetProperty(
                propertyName,
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetProperty);

            if (propertyInfo == null)
            {
                throw new ArgumentException(
                    $"Could not find property info of name {propertyName} in type {typeof(ForgettablePayload)}");
            }

            return propertyInfo;
        }
    }
}