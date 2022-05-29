using System;
using System.Collections.Generic;
using System.Linq;
using EventSourcing.ForgettablePayloads.Abstractions;
using EventSourcing.ForgettablePayloads.Abstractions.ValueObjects;
using EventSourcing.ForgettablePayloads.Services;
using TestHelpers.Attributes;
using Xunit;
using Enumerable = System.Linq.Enumerable;

namespace ForgettablePayloads.UnitTests.Services
{
    public class ForgettablePayloadFinder_Should
    {
        #region Test ForgettablePayload input objects
        
        [Theory]
        [AutoMoqData]
        internal void Throw_ArgumentNullException_When_Finding_And_EventIsNull(
            ForgettablePayloadFinder finder)
        {
            Assert.Throws<ArgumentNullException>(() => finder.Find(null));
        }

        [Theory]
        [AutoMoqData]
        internal void ReturnEmptyCollection_When_Finding_And_EventIsObject(
            object @event,
            ForgettablePayloadFinder finder)
        {
            var result = finder.Find(@event);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Theory]
        [AutoMoqData]
        internal void ReturnForgettablePayload_When_Finding_And_EventIsOfForgettablePayloadType(
            ForgettablePayload forgettablePayload,
            ForgettablePayloadFinder payloadFinder)
        {
            var result = payloadFinder.Find(forgettablePayload);

            var singleForgettablePayload = Assert.Single(result);
            Assert.Equal(forgettablePayload, singleForgettablePayload);
        }

        [Theory]
        [AutoMoqData]
        internal void ReturnForgettablePayload_When_Finding_And_EventIsOfForgettablePayloadGenericType(
            ForgettablePayload<object> forgettablePayload,
            ForgettablePayloadFinder payloadFinder)
        {
            var result = payloadFinder.Find(forgettablePayload);

            var singleForgettablePayload = Assert.Single(result);
            Assert.Equal(forgettablePayload, singleForgettablePayload);
        }

        [Theory]
        [AutoMoqData]
        internal void ReturnForgettablePayload_When_Finding_And_EventIsOfTypeInheritingFromForgettablePayloadType(
            ForgettablePayloadFinder payloadFinder)
        {
            var forgettablePayload = new ForgettablePayloadInheritance(ForgettablePayloadId.NewForgettablePayloadId());
            
            var result = payloadFinder.Find(forgettablePayload);

            var singleForgettablePayload = Assert.Single(result);
            Assert.Equal(forgettablePayload, singleForgettablePayload);
        }

        [Theory]
        [AutoMoqData]
        internal void ReturnForgettablePayload_When_Finding_And_EventIsOfTypeInheritingFromForgettablePayloadGenericType(
            ForgettablePayloadFinder payloadFinder)
        {
            var forgettablePayload = new ForgettablePayloadGenericInheritance<object>(ForgettablePayloadId.NewForgettablePayloadId());
            
            var result = payloadFinder.Find(forgettablePayload);

            var singleForgettablePayload = Assert.Single(result);
            Assert.Equal(forgettablePayload, singleForgettablePayload);
        }

        [Theory]
        [AutoMoqData]
        internal void ReturnForgettablePayloads_When_Finding_And_EventIsOfForgettablePayloadCollectionType(
            List<ForgettablePayload> forgettablePayloads,
            ForgettablePayloadFinder payloadFinder)
        {
            var result = payloadFinder.Find(forgettablePayloads);

            Assert.Equal(forgettablePayloads.Count, result.Count);
            Assert.All(forgettablePayloads, payload => Assert.Contains(payload, result));
        }

        [Theory]
        [AutoMoqData]
        internal void ReturnForgettablePayloads_When_Finding_And_EventIsOfForgettablePayloadInheritanceCollectionType(
            ForgettablePayloadFinder payloadFinder)
        {
            var forgettablePayloads = Enumerable.Range(0, 3)
                .Select(i => new ForgettablePayloadInheritance(ForgettablePayloadId.NewForgettablePayloadId()))
                .ToList();
            
            var result = payloadFinder.Find(forgettablePayloads);

            Assert.Equal(forgettablePayloads.Count, result.Count);
            Assert.All(forgettablePayloads, payload => Assert.Contains(payload, result));
        }

        [Theory]
        [AutoMoqData]
        internal void ReturnForgettablePayloads_When_Finding_And_EventIsOfTypeContainingForgettablePayloadPublicPropertyWithPublicGetterCollectionType(
            ForgettablePayloadFinder payloadFinder)
        {
            var forgettablePayloads = Enumerable.Range(0, 3)
                .Select(i => new ClassWithPublicForgettablePayloadProperty
                {
                    PublicForgettablePayloadProperty = new ForgettablePayload(ForgettablePayloadId.NewForgettablePayloadId())
                })
                .ToList();
            
            var result = payloadFinder.Find(forgettablePayloads);

            Assert.Equal(forgettablePayloads.Count, result.Count);
            Assert.All(forgettablePayloads, payload => Assert.Contains(payload.PublicForgettablePayloadProperty, result));
        }

        [Theory]
        [AutoMoqData]
        internal void ReturnForgettablePayloads_When_Finding_And_EventIsOfTypeContainingForgettablePayloadPublicFieldCollectionType(
            ForgettablePayloadFinder payloadFinder)
        {
            var forgettablePayloads = Enumerable.Range(0, 3)
                .Select(i => new ClassWithPublicForgettablePayloadField
                {
                    PublicForgettablePayloadField = new ForgettablePayload(ForgettablePayloadId.NewForgettablePayloadId())
                })
                .ToList();
            
            var result = payloadFinder.Find(forgettablePayloads);

            Assert.Equal(forgettablePayloads.Count, result.Count);
            Assert.All(forgettablePayloads, payload => Assert.Contains(payload.PublicForgettablePayloadField, result));
        }

        [Theory]
        [AutoMoqData]
        internal void ReturnForgettablePayloads_When_Finding_And_EventIsOfForgettablePayloadCollectionTypeWithPublicPropertyAndFieldOfForgettablePayloadType(
            List<ForgettablePayload> forgettablePayloads,
            ForgettablePayload forgettablePayloadField,
            ForgettablePayload forgettablePayloadProperty,
            ForgettablePayloadFinder payloadFinder)
        {
            var @event = new CollectionClassOfForgettablePayloadWithPublicForgettablePayloadFieldAndPublicForgettablePayloadPropertyWithPublicGetter();
            @event.AddRange(forgettablePayloads);
            @event.PublicForgettablePayloadField = forgettablePayloadField;
            @event.PublicForgettablePayloadProperty = forgettablePayloadProperty;
            
            var result = payloadFinder.Find(@event);

            Assert.Equal(forgettablePayloads.Count + 2, result.Count);
            Assert.All(forgettablePayloads, payload => Assert.Contains(payload, result));
            Assert.Contains(forgettablePayloadField, result);
            Assert.Contains(forgettablePayloadProperty, result);
        }

        [Theory]
        [AutoMoqData]
        internal void ReturnForgettablePayloads_When_Finding_And_EventIsOfTypeContainingForgettablePayloadPublicPropertyWithPublicGetterCollectionTypeWithPublicPropertyAndFieldOfForgettablePayloadType(
            ForgettablePayload forgettablePayloadField,
            ForgettablePayload forgettablePayloadProperty,
            ForgettablePayloadFinder payloadFinder)
        {
            var forgettablePayloads = Enumerable.Range(0, 3)
                .Select(i => new ClassWithPublicForgettablePayloadProperty
                {
                    PublicForgettablePayloadProperty = new ForgettablePayload(ForgettablePayloadId.NewForgettablePayloadId())
                })
                .ToList();
            
            var @event = new CollectionClassOfClassWithPublicForgettablePayloadPropertyWithPublicForgettablePayloadFieldAndPublicForgettablePayloadPropertyWithPublicGetter();
            @event.AddRange(forgettablePayloads);
            @event.PublicForgettablePayloadField = forgettablePayloadField;
            @event.PublicForgettablePayloadProperty = forgettablePayloadProperty;
            
            var result = payloadFinder.Find(@event);

            Assert.Equal(forgettablePayloads.Count + 2, result.Count);
            Assert.All(forgettablePayloads, payload => Assert.Contains(payload.PublicForgettablePayloadProperty, result));
            Assert.Contains(forgettablePayloadField, result);
            Assert.Contains(forgettablePayloadProperty, result);
        }

        [Theory]
        [AutoMoqData]
        internal void ReturnForgettablePayloads_When_Finding_And_EventIsOfTypeContainingForgettablePayloadPublicFieldCollectionTypeWithPublicPropertyAndFieldOfForgettablePayloadType(
            ForgettablePayload forgettablePayloadField,
            ForgettablePayload forgettablePayloadProperty,
            ForgettablePayloadFinder payloadFinder)
        {
            var forgettablePayloads = Enumerable.Range(0, 3)
                .Select(i => new ClassWithPublicForgettablePayloadField
                {
                    PublicForgettablePayloadField = new ForgettablePayload(ForgettablePayloadId.NewForgettablePayloadId())
                })
                .ToList();
            
            var @event = new CollectionClassOfClassWithPublicForgettablePayloadFieldWithPublicForgettablePayloadFieldAndPublicForgettablePayloadPropertyWithPublicGetter();
            @event.AddRange(forgettablePayloads);
            @event.PublicForgettablePayloadField = forgettablePayloadField;
            @event.PublicForgettablePayloadProperty = forgettablePayloadProperty;
            
            var result = payloadFinder.Find(@event);

            Assert.Equal(forgettablePayloads.Count + 2, result.Count);
            Assert.All(forgettablePayloads, payload => Assert.Contains(payload.PublicForgettablePayloadField, result));
            Assert.Contains(forgettablePayloadField, result);
            Assert.Contains(forgettablePayloadProperty, result);
        }

        #endregion

        #region Recursion

        #region Test Properties

        [Theory]
        [AutoMoqData]
        internal void Throw_When_Finding_And_EventIsOfTypeInheritingFromForgettablePayloadTypeAndHasPublicPropertyWithPublicGetterOfForgettablePayloadType(
            ForgettablePayload nestedForgettablePayload,
            ForgettablePayloadFinder payloadFinder)
        {
            var forgettablePayload = new ForgettablePayloadInheritanceWithPublicForgettablePayloadProperty(ForgettablePayloadId.NewForgettablePayloadId())
            {
                PublicForgettablePayloadProperty = nestedForgettablePayload
            };
            
            Assert.Throws<InvalidOperationException>(() => payloadFinder.Find(forgettablePayload));
        }

        [Theory]
        [AutoMoqData]
        internal void Throw_When_Finding_And_EventIsOfTypeInheritingFromForgettablePayloadTypeAndHasPublicPropertyWithPublicGetterOfTypeInheritingFromForgettablePayloadType(
            ForgettablePayloadFinder payloadFinder)
        {
            var nestedProperty = new ForgettablePayloadInheritance(ForgettablePayloadId.NewForgettablePayloadId());
            var forgettablePayload = new ForgettablePayloadInheritanceWithPublicForgettablePayloadInheritanceProperty(ForgettablePayloadId.NewForgettablePayloadId())
            {
                PublicForgettablePayloadInheritanceProperty = nestedProperty
            };
            
            Assert.Throws<InvalidOperationException>(() => payloadFinder.Find(forgettablePayload));
        }

        [Theory]
        [AutoMoqData]
        internal void ReturnForgettablePayload_When_Finding_And_EventHasPublicPropertyOfForgettablePayloadType(
            ForgettablePayload forgettablePayload,
            ForgettablePayloadFinder payloadFinder)
        {
            var @event = new ClassWithPublicForgettablePayloadProperty
            {
                PublicForgettablePayloadProperty = forgettablePayload
            };

            var result = payloadFinder.Find(@event);

            var singleForgettablePayload = Assert.Single(result);
            Assert.Equal(forgettablePayload, singleForgettablePayload);
        }

        [Theory]
        [AutoMoqData]
        internal void ReturnForgettablePayload_When_Finding_And_EventHasPublicPropertyOfForgettablePayloadGenericType(
            ForgettablePayload<object> forgettablePayload,
            ForgettablePayloadFinder payloadFinder)
        {
            var @event = new ClassWithPublicForgettablePayloadGenericProperty
            {
                PublicForgettablePayloadGenericProperty = forgettablePayload
            };

            var result = payloadFinder.Find(@event);

            var singleForgettablePayload = Assert.Single(result);
            Assert.Equal(forgettablePayload, singleForgettablePayload);
        }

        [Theory]
        [AutoMoqData]
        internal void ReturnEmptyCollection_When_Finding_And_EventHasPublicPropertyOfForgettablePayloadType_And_PropertyValueIsNull(
            ForgettablePayloadFinder payloadFinder)
        {
            var @event = new ClassWithPublicForgettablePayloadProperty
            {
                PublicForgettablePayloadProperty = null
            };

            var result = payloadFinder.Find(@event);

            Assert.Empty(result);
        }

        [Theory]
        [AutoMoqData]
        internal void ReturnEmptyCollection_When_Finding_And_EventHasPublicStaticPropertyOfForgettablePayloadType(
            ForgettablePayload forgettablePayload,
            ForgettablePayloadFinder payloadFinder)
        {
            var @event = new ClassWithPublicStaticForgettablePayloadProperty();
            ClassWithPublicStaticForgettablePayloadProperty.PublicStaticForgettablePayloadProperty = forgettablePayload;

            var result = payloadFinder.Find(@event);

            Assert.Empty(result);
        }

        [Theory]
        [AutoMoqData]
        internal void ReturnEmptyCollection_When_Finding_And_EventHasPublicSetOnlyPropertyOfForgettablePayloadType(
            ForgettablePayload forgettablePayload,
            ForgettablePayloadFinder payloadFinder)
        {
            var @event = new ClassWithPublicForgettablePayloadSetOnlyProperty
            {
                PublicForgettablePayloadSetOnlyProperty = forgettablePayload
            };

            var result = payloadFinder.Find(@event);

            Assert.Empty(result);
        }

        [Theory]
        [AutoMoqData]
        internal void ReturnEmptyCollection_When_Finding_And_EventHasPublicPropertyWithPublicSetterAndPrivateGetterOfForgettablePayloadType(
            ForgettablePayload forgettablePayload,
            ForgettablePayloadFinder payloadFinder)
        {
            var @event = new ClassWithPublicForgettablePayloadWithPublicSetterAndPrivateGetterProperty
            {
                PublicForgettablePayloadWithPublicSetterAndPrivateGetterProperty = forgettablePayload
            };

            var result = payloadFinder.Find(@event);

            Assert.Empty(result);
        }

        [Theory]
        [AutoMoqData]
        internal void ReturnEmptyCollection_When_Finding_And_EventHasInternalPropertyOfForgettablePayloadType(
            ForgettablePayload forgettablePayload,
            ForgettablePayloadFinder payloadFinder)
        {
            var @event = new ClassWithInternalForgettablePayloadProperty
            {
                InternalForgettablePayloadProperty = forgettablePayload
            };

            var result = payloadFinder.Find(@event);

            Assert.Empty(result);
        }

        [Theory]
        [AutoMoqData]
        internal void ReturnEmptyCollection_When_Finding_And_EventHasPrivatePropertyOfForgettablePayloadType(
            ForgettablePayload forgettablePayload,
            ForgettablePayloadFinder payloadFinder)
        {
            var @event = new ClassWithPrivateForgettablePayloadProperty(forgettablePayload);

            var result = payloadFinder.Find(@event);

            Assert.Empty(result);
        }

        [Theory]
        [AutoMoqData]
        internal void ReturnForgettablePayload_When_Finding_And_EventHasPublicPropertyOfTypeThatHasPublicPropertyOfForgettablePayloadType(
            ForgettablePayload forgettablePayload,
            ForgettablePayloadFinder payloadFinder)
        {
            var @event = new ClassWithClassWithPublicForgettablePayloadProperty
            {
                PublicClassWithPublicForgettablePayloadProperty = new ClassWithPublicForgettablePayloadProperty
                {
                    PublicForgettablePayloadProperty = forgettablePayload
                }
            };

            var result = payloadFinder.Find(@event);

            var singleForgettablePayload = Assert.Single(result);
            Assert.Equal(forgettablePayload, singleForgettablePayload);
        }

        [Theory]
        [AutoMoqData]
        internal void ReturnForgettablePayload_When_Finding_And_EventHasPublicGetOnlyPropertyOfForgettablePayloadType(
            ForgettablePayload forgettablePayload,
            ForgettablePayloadFinder payloadFinder)
        {
            var @event = new ClassWithPublicForgettablePayloadGetOnlyProperty(forgettablePayload);

            var result = payloadFinder.Find(@event);

            var singleForgettablePayload = Assert.Single(result);
            Assert.Equal(forgettablePayload, singleForgettablePayload);
        }

        #endregion

        #region Test Fields

        [Theory]
        [AutoMoqData]
        internal void Throw_When_Finding_And_EventIsOfTypeInheritingFromForgettablePayloadTypeAndHasPublicFieldOfForgettablePayloadType(
            ForgettablePayload nestedForgettablePayload,
            ForgettablePayloadFinder payloadFinder)
        {
            var forgettablePayload = new ForgettablePayloadInheritanceWithPublicForgettablePayloadField(ForgettablePayloadId.NewForgettablePayloadId())
            {
                PublicForgettablePayloadField = nestedForgettablePayload
            };
            
            Assert.Throws<InvalidOperationException>(() => payloadFinder.Find(forgettablePayload));
        }

        [Theory]
        [AutoMoqData]
        internal void Throw_When_Finding_And_EventIsOfTypeInheritingFromForgettablePayloadTypeAndHasPublicFieldOfTypeInheritingFromForgettablePayloadType(
            ForgettablePayloadFinder payloadFinder)
        {
            var nestedProperty = new ForgettablePayloadInheritance(ForgettablePayloadId.NewForgettablePayloadId());
            var forgettablePayload = new ForgettablePayloadInheritanceWithPublicForgettablePayloadInheritanceField(ForgettablePayloadId.NewForgettablePayloadId())
            {
                PublicForgettablePayloadInheritanceField = nestedProperty
            };
            
            Assert.Throws<InvalidOperationException>(() => payloadFinder.Find(forgettablePayload));
        }

        [Theory]
        [AutoMoqData]
        internal void ReturnForgettablePayload_When_Finding_And_EventHasPublicFieldOfForgettablePayloadType(
            ForgettablePayload forgettablePayload,
            ForgettablePayloadFinder payloadFinder)
        {
            var @event = new ClassWithPublicForgettablePayloadField
            {
                PublicForgettablePayloadField = forgettablePayload
            };

            var result = payloadFinder.Find(@event);

            var singleForgettablePayload = Assert.Single(result);
            Assert.Equal(forgettablePayload, singleForgettablePayload);
        }

        [Theory]
        [AutoMoqData]
        internal void ReturnForgettablePayload_When_Finding_And_EventHasPublicFieldOfForgettablePayloadGenericType(
            ForgettablePayload<object> forgettablePayload,
            ForgettablePayloadFinder payloadFinder)
        {
            var @event = new ClassWithPublicForgettablePayloadGenericField
            {
                PublicForgettablePayloadGenericField = forgettablePayload
            };

            var result = payloadFinder.Find(@event);

            var singleForgettablePayload = Assert.Single(result);
            Assert.Equal(forgettablePayload, singleForgettablePayload);
        }

        [Theory]
        [AutoMoqData]
        internal void ReturnEmptyCollection_When_Finding_And_EventHasPublicFieldOfForgettablePayloadType_And_FieldValueIsNull(
            ForgettablePayloadFinder payloadFinder)
        {
            var @event = new ClassWithPublicForgettablePayloadField
            {
                PublicForgettablePayloadField = null
            };

            var result = payloadFinder.Find(@event);

            Assert.Empty(result);
        }

        [Theory]
        [AutoMoqData]
        internal void ReturnEmptyCollection_When_Finding_And_EventHasPublicStaticFieldOfForgettablePayloadType(
            ForgettablePayload forgettablePayload,
            ForgettablePayloadFinder payloadFinder)
        {
            var @event = new ClassWithPublicStaticForgettablePayloadField();
            ClassWithPublicStaticForgettablePayloadField.PublicStaticForgettablePayloadField = forgettablePayload;

            var result = payloadFinder.Find(@event);

            Assert.Empty(result);
        }

        [Theory]
        [AutoMoqData]
        internal void ReturnEmptyCollection_When_Finding_And_EventHasInternalFieldOfForgettablePayloadType(
            ForgettablePayload forgettablePayload,
            ForgettablePayloadFinder payloadFinder)
        {
            var @event = new ClassWithInternalForgettablePayloadField
            {
                InternalForgettablePayloadField = forgettablePayload
            };

            var result = payloadFinder.Find(@event);

            Assert.Empty(result);
        }

        [Theory]
        [AutoMoqData]
        internal void ReturnEmptyCollection_When_Finding_And_EventHasPrivateFieldOfForgettablePayloadType(
            ForgettablePayload forgettablePayload,
            ForgettablePayloadFinder payloadFinder)
        {
            var @event = new ClassWithPrivateForgettablePayloadField(forgettablePayload);

            var result = payloadFinder.Find(@event);

            Assert.Empty(result);
        }

        [Theory]
        [AutoMoqData]
        internal void ReturnForgettablePayload_When_Finding_And_EventHasPublicFieldOfTypeThatHasPublicFieldOfForgettablePayloadType(
            ForgettablePayload forgettablePayload,
            ForgettablePayloadFinder payloadFinder)
        {
            var @event = new ClassWithClassWithPublicForgettablePayloadField
            {
                PublicClassWithPublicForgettablePayloadField = new ClassWithPublicForgettablePayloadField
                {
                    PublicForgettablePayloadField = forgettablePayload
                }
            };

            var result = payloadFinder.Find(@event);

            var singleForgettablePayload = Assert.Single(result);
            Assert.Equal(forgettablePayload, singleForgettablePayload);
        }

        #endregion

        #endregion

        #region Classes for tests purposes
        
        private class ClassWithClassWithPublicForgettablePayloadProperty
        {
            public ClassWithPublicForgettablePayloadProperty PublicClassWithPublicForgettablePayloadProperty { get; set; }
        }
        
        private class ClassWithClassWithPublicForgettablePayloadField
        {
            public ClassWithPublicForgettablePayloadField PublicClassWithPublicForgettablePayloadField;
        }
        
        private class ClassWithPublicForgettablePayloadGetOnlyProperty
        {
            public ForgettablePayload PublicForgettablePayloadProperty { get; }

            public ClassWithPublicForgettablePayloadGetOnlyProperty(ForgettablePayload forgettablePayload)
            {
                PublicForgettablePayloadProperty = forgettablePayload;
            }
        }
        
        private class ClassWithPublicForgettablePayloadProperty
        {
            public ForgettablePayload PublicForgettablePayloadProperty { get; set; }
        }
        
        private class ClassWithPublicForgettablePayloadGenericProperty
        {
            public ForgettablePayload<object> PublicForgettablePayloadGenericProperty { get; set; }
        }
        
        private class ClassWithPublicStaticForgettablePayloadProperty
        {
            public static ForgettablePayload PublicStaticForgettablePayloadProperty { get; set; }
        }
        
        private class ClassWithPublicForgettablePayloadSetOnlyProperty
        {
            private ForgettablePayload _forgettablePayload;
            
            public ForgettablePayload PublicForgettablePayloadSetOnlyProperty
            {
                set => _forgettablePayload = value;
            }
        }
        
        private class ClassWithPublicForgettablePayloadWithPublicSetterAndPrivateGetterProperty
        {
            public ForgettablePayload PublicForgettablePayloadWithPublicSetterAndPrivateGetterProperty
            {
                private get;
                set;
            }
        }
        
        private class ClassWithInternalForgettablePayloadProperty
        {
            internal ForgettablePayload InternalForgettablePayloadProperty { get; set; }
        }
        
        private class ClassWithPrivateForgettablePayloadProperty
        {
            private ForgettablePayload PrivateForgettablePayloadProperty { get; set; }

            public ClassWithPrivateForgettablePayloadProperty(ForgettablePayload forgettablePayload)
            {
                PrivateForgettablePayloadProperty = forgettablePayload;
            }
        }
        
        private class ForgettablePayloadInheritance : ForgettablePayload
        {
            public ForgettablePayloadInheritance(ForgettablePayloadId payloadId) : base(payloadId)
            {
            }
        }
        
        private class ForgettablePayloadGenericInheritance<T> : ForgettablePayload<T>
        {
            public ForgettablePayloadGenericInheritance(ForgettablePayloadId payloadId) : base(payloadId)
            {
            }
        }
        
        private class ForgettablePayloadInheritanceWithPublicForgettablePayloadProperty : ForgettablePayload
        {
            public ForgettablePayload PublicForgettablePayloadProperty { get; set; }
            
            public ForgettablePayloadInheritanceWithPublicForgettablePayloadProperty(ForgettablePayloadId payloadId) : base(payloadId)
            {
            }
        }
        
        private class ForgettablePayloadInheritanceWithPublicForgettablePayloadInheritanceProperty : ForgettablePayload
        {
            public ForgettablePayloadInheritance PublicForgettablePayloadInheritanceProperty { get; set; }
            
            public ForgettablePayloadInheritanceWithPublicForgettablePayloadInheritanceProperty(ForgettablePayloadId payloadId) : base(payloadId)
            {
            }
        }
        
        private class ForgettablePayloadInheritanceWithPublicForgettablePayloadField : ForgettablePayload
        {
            public ForgettablePayload PublicForgettablePayloadField;
            
            public ForgettablePayloadInheritanceWithPublicForgettablePayloadField(ForgettablePayloadId payloadId) : base(payloadId)
            {
            }
        }
        
        private class ForgettablePayloadInheritanceWithPublicForgettablePayloadInheritanceField : ForgettablePayload
        {
            public ForgettablePayloadInheritance PublicForgettablePayloadInheritanceField;
            
            public ForgettablePayloadInheritanceWithPublicForgettablePayloadInheritanceField(ForgettablePayloadId payloadId) : base(payloadId)
            {
            }
        }
        
        private class ClassWithPublicForgettablePayloadField
        {
            public ForgettablePayload PublicForgettablePayloadField;
        }
        
        private class ClassWithPublicForgettablePayloadGenericField
        {
            public ForgettablePayload<object> PublicForgettablePayloadGenericField;
        }
        
        private class ClassWithPublicStaticForgettablePayloadField
        {
            public static ForgettablePayload PublicStaticForgettablePayloadField;
        }
        
        private class ClassWithInternalForgettablePayloadField
        {
            internal ForgettablePayload InternalForgettablePayloadField;
        }
        
        private class ClassWithPrivateForgettablePayloadField
        {
            private ForgettablePayload PrivateForgettablePayloadField;

            public ClassWithPrivateForgettablePayloadField(ForgettablePayload forgettablePayload)
            {
                PrivateForgettablePayloadField = forgettablePayload;
            }
        }
        
        private class CollectionClassOfForgettablePayloadWithPublicForgettablePayloadFieldAndPublicForgettablePayloadPropertyWithPublicGetter : List<ForgettablePayload>
        {
            public ForgettablePayload PublicForgettablePayloadField;
            
            public ForgettablePayload PublicForgettablePayloadProperty { get; set; }
        }
        
        private class CollectionClassOfClassWithPublicForgettablePayloadPropertyWithPublicForgettablePayloadFieldAndPublicForgettablePayloadPropertyWithPublicGetter : List<ClassWithPublicForgettablePayloadProperty>
        {
            public ForgettablePayload PublicForgettablePayloadField;
            
            public ForgettablePayload PublicForgettablePayloadProperty { get; set; }
        }
        
        private class CollectionClassOfClassWithPublicForgettablePayloadFieldWithPublicForgettablePayloadFieldAndPublicForgettablePayloadPropertyWithPublicGetter : List<ClassWithPublicForgettablePayloadField>
        {
            public ForgettablePayload PublicForgettablePayloadField;
            
            public ForgettablePayload PublicForgettablePayloadProperty { get; set; }
        }
        
        #endregion
    }
}