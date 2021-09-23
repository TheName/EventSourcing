using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using EventSourcing.Bus.RabbitMQ.Abstractions.Configurations;
using EventSourcing.Bus.RabbitMQ.Channels;
using EventSourcing.Serialization.Abstractions;
using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TestHelpers.Attributes;
using Xunit;

namespace Bus.RabbitMQ.UnitTests.Channels
{
    public class RabbitMQConsumingChannel_Should
    {
        [Theory]
        [AutoMoqData]
        internal void Throw_ArgumentNullException_When_Creating_And_ConnectionIsNull(
            IRabbitMQConsumingChannelConfiguration consumingChannelConfiguration,
            ISerializer serializer,
            ILogger<RabbitMQConsumingChannel> logger)
        {
            Assert.Throws<ArgumentNullException>(() => new RabbitMQConsumingChannel(
                null,
                consumingChannelConfiguration,
                serializer,
                logger));
        }
        
        [Theory]
        [AutoMoqData]
        internal void Throw_ArgumentNullException_When_Creating_And_ConsumingChannelConfigurationIsNull(
            IConnection connection,
            ISerializer serializer,
            ILogger<RabbitMQConsumingChannel> logger)
        {
            Assert.Throws<ArgumentNullException>(() => new RabbitMQConsumingChannel(
                connection,
                null,
                serializer,
                logger));
        }
        
        [Theory]
        [AutoMoqData]
        internal void Throw_ArgumentNullException_When_Creating_And_SerializerIsNull(
            IConnection connection,
            IRabbitMQConsumingChannelConfiguration consumingChannelConfiguration,
            ILogger<RabbitMQConsumingChannel> logger)
        {
            Assert.Throws<ArgumentNullException>(() => new RabbitMQConsumingChannel(
                connection,
                consumingChannelConfiguration,
                null,
                logger));
        }
        
        [Theory]
        [AutoMoqData]
        internal void Throw_ArgumentNullException_When_Creating_And_LoggerIsNull(
            IConnection connection,
            IRabbitMQConsumingChannelConfiguration consumingChannelConfiguration,
            ISerializer serializer)
        {
            Assert.Throws<ArgumentNullException>(() => new RabbitMQConsumingChannel(
                connection,
                consumingChannelConfiguration,
                serializer,
                null));
        }
        
        [Theory]
        [AutoMoqData]
        internal void NotThrow_When_Creating_And_AllParametersAreNotNull(
            IConnection connection,
            IRabbitMQConsumingChannelConfiguration consumingChannelConfiguration,
            ISerializer serializer,
            ILogger<RabbitMQConsumingChannel> logger)
        {
            _ = new RabbitMQConsumingChannel(
                connection,
                consumingChannelConfiguration,
                serializer,
                logger);
        }

        [Theory]
        [AutoMoqData]
        internal void NotCreateConsumingChannelInConstructor(
            Mock<IConnection> connectionMock,
            IRabbitMQConsumingChannelConfiguration consumingChannelConfiguration,
            ISerializer serializer,
            ILogger<RabbitMQConsumingChannel> logger)
        {
            _ = new RabbitMQConsumingChannel(
                connectionMock.Object,
                consumingChannelConfiguration,
                serializer,
                logger);
            
            connectionMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal void CreateConsumingChannel_When_AddingFirstConsumer(
            Func<object, CancellationToken, Task> handler,
            CancellationToken cancellationToken,
            [Frozen] Mock<IConnection> connectionMock,
            RabbitMQConsumingChannel consumingChannel)
        {
            connectionMock.VerifyNoOtherCalls();
            
            consumingChannel.AddConsumer(handler, cancellationToken);
            
            connectionMock.Verify(connection => connection.CreateModel(), Times.Once);
        }

        [Theory]
        [AutoMoqWithInlineData(3)]
        [AutoMoqWithInlineData(10)]
        [AutoMoqWithInlineData(100)]
        internal void NotCreateConsumingChannel_When_AddingNextConsumerAfterTheFirstOne(
            int numberOfAddedConsumersAfterTheFirstOne,
            Func<object, CancellationToken, Task> handler,
            CancellationToken cancellationToken,
            [Frozen] Mock<IConnection> connectionMock,
            RabbitMQConsumingChannel consumingChannel)
        {
            consumingChannel.AddConsumer(handler, cancellationToken);
            connectionMock.Reset();
            
            Enumerable.Range(0, numberOfAddedConsumersAfterTheFirstOne).ToList()
                .ForEach(i => consumingChannel.AddConsumer(handler, cancellationToken));
            
            connectionMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal void ProperlyConfigureCreatedConsumingChannel_When_AddingFirstConsumer(
            string exchangeName,
            string exchangeType,
            string queueName,
            string routingKey,
            ushort prefetchCount,
            Func<object, CancellationToken, Task> handler,
            CancellationToken cancellationToken,
            Mock<IModel> consumingChannelMock,
            [Frozen] Mock<IConnection> connectionMock,
            [Frozen] Mock<IRabbitMQConsumingChannelConfiguration> consumingChannelConfigurationMock,
            RabbitMQConsumingChannel consumingChannel)
        {
            connectionMock
                .Setup(connection => connection.CreateModel())
                .Returns(consumingChannelMock.Object);

            consumingChannelConfigurationMock
                .SetupGet(configuration => configuration.ExchangeName)
                .Returns(exchangeName);

            consumingChannelConfigurationMock
                .SetupGet(configuration => configuration.ExchangeType)
                .Returns(exchangeType);

            consumingChannelConfigurationMock
                .SetupGet(configuration => configuration.QueueName)
                .Returns(queueName);

            consumingChannelConfigurationMock
                .SetupGet(configuration => configuration.RoutingKey)
                .Returns(routingKey);

            consumingChannelConfigurationMock
                .SetupGet(configuration => configuration.PrefetchCount)
                .Returns(prefetchCount);
            
            consumingChannel.AddConsumer(handler, cancellationToken);

            consumingChannelMock.Verify(model => model.ExchangeDeclare(exchangeName, exchangeType, true, false, null), Times.Once);
            consumingChannelMock.Verify(model => model.QueueDeclare(queueName, true, false, false, null), Times.Once);
            consumingChannelMock.Verify(model => model.QueueBind(queueName, exchangeName, routingKey, null), Times.Once);
            consumingChannelMock.Verify(model => model.BasicQos(0, prefetchCount, false), Times.Once);
        }

        [Theory]
        [AutoMoqWithInlineData(3)]
        [AutoMoqWithInlineData(10)]
        [AutoMoqWithInlineData(100)]
        internal void NotConfigureCreatedConsumingChannel_When_AddingNextConsumerAfterTheFirstOne(
            int numberOfAddedConsumersAfterTheFirstOne,
            Func<object, CancellationToken, Task> handler,
            CancellationToken cancellationToken,
            Mock<IModel> consumingChannelMock,
            [Frozen] Mock<IConnection> connectionMock,
            RabbitMQConsumingChannel consumingChannel)
        {
            connectionMock
                .Setup(connection => connection.CreateModel())
                .Returns(consumingChannelMock.Object);
            
            consumingChannel.AddConsumer(handler, cancellationToken);
            consumingChannelMock.Reset();
            
            Enumerable.Range(0, numberOfAddedConsumersAfterTheFirstOne).ToList()
                .ForEach(i => consumingChannel.AddConsumer(handler, cancellationToken));
            
            consumingChannelMock.Verify(model => model.ExchangeDeclare(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<IDictionary<string,object>>()), Times.Never);
            consumingChannelMock.Verify(model => model.QueueDeclare(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<IDictionary<string,object>>()), Times.Never);
            consumingChannelMock.Verify(model => model.QueueBind(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IDictionary<string,object>>()), Times.Never);
            consumingChannelMock.Verify(model => model.BasicQos(It.IsAny<uint>(), It.IsAny<ushort>(), It.IsAny<bool>()), Times.Never);
        }

        [Theory]
        [AutoMoqWithInlineData(3)]
        [AutoMoqWithInlineData(10)]
        [AutoMoqWithInlineData(100)]
        internal void AddAsyncEventingBasicConsumer_When_AddingConsumerEachTime(
            int numberOfAddedConsumersAfterTheFirstOne,
            string queueName,
            Func<object, CancellationToken, Task> handler,
            CancellationToken cancellationToken,
            Mock<IModel> consumingChannelMock,
            [Frozen] Mock<IConnection> connectionMock,
            [Frozen] Mock<IRabbitMQConsumingChannelConfiguration> consumingChannelConfigurationMock,
            RabbitMQConsumingChannel consumingChannel)
        {
            connectionMock
                .Setup(connection => connection.CreateModel())
                .Returns(consumingChannelMock.Object);

            consumingChannelConfigurationMock
                .SetupGet(configuration => configuration.QueueName)
                .Returns(queueName);
            
            Enumerable.Range(0, numberOfAddedConsumersAfterTheFirstOne).ToList()
                .ForEach(i => consumingChannel.AddConsumer(handler, cancellationToken));

            var assertConsumer = new Func<IBasicConsumer, bool>(consumer =>
            {
                var asyncEventingBasicConsumer = Assert.IsType<AsyncEventingBasicConsumer>(consumer);
                Assert.Equal(consumingChannelMock.Object, asyncEventingBasicConsumer.Model);
                
                return true;
            });

            consumingChannelMock.Verify(
                model => model.BasicConsume(
                    queueName, 
                    false,
                    string.Empty,
                    false,
                    false,
                    null,
                    It.Is<IBasicConsumer>(consumer => assertConsumer(consumer))),
                Times.Exactly(numberOfAddedConsumersAfterTheFirstOne));
        }

        [Theory]
        [AutoMoqWithInlineData(3)]
        [AutoMoqWithInlineData(10)]
        [AutoMoqWithInlineData(100)]
        internal void CancelEveryAddedConsumer_When_DisposingAfterAddingConsumers(
            int numberOfAddedConsumersAfterTheFirstOne,
            string queueName,
            Func<object, CancellationToken, Task> handler,
            CancellationToken cancellationToken,
            Mock<IModel> consumingChannelMock,
            [Frozen] Mock<IConnection> connectionMock,
            [Frozen] Mock<IRabbitMQConsumingChannelConfiguration> consumingChannelConfigurationMock,
            RabbitMQConsumingChannel consumingChannel)
        {
            // Arrange
            connectionMock
                .Setup(connection => connection.CreateModel())
                .Returns(consumingChannelMock.Object);

            consumingChannelConfigurationMock
                .SetupGet(configuration => configuration.QueueName)
                .Returns(queueName);

            var consumerTags = Enumerable.Range(0, numberOfAddedConsumersAfterTheFirstOne)
                .Select(i => Guid.NewGuid().ToString())
                .ToList();

            var setupSequence = consumingChannelMock
                .SetupSequence(model => model.BasicConsume(
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<IDictionary<string, object>>(),
                    It.IsAny<IBasicConsumer>()));
            
            consumerTags.ForEach(consumerTag => setupSequence.Returns(consumerTag));
            
            // Act
            Enumerable.Range(0, numberOfAddedConsumersAfterTheFirstOne).ToList()
                .ForEach(i => consumingChannel.AddConsumer(handler, cancellationToken));
            
            consumingChannel.Dispose();
            
            // Assert
            foreach (var consumerTag in consumerTags)
            {
                consumingChannelMock.Verify(model => model.BasicCancel(consumerTag), Times.Once);
            }
        }

        [Theory]
        [AutoMoqData]
        internal async Task DeserializeBodyFromUtf8BytesToEventStreamEntry_When_HandlingMessage(
            string consumingTag,
            ulong deliveryTag,
            bool redelivered,
            string exchange,
            string routingKey,
            IBasicProperties basicProperties,
            ReadOnlyMemory<byte> body,
            Func<object, CancellationToken, Task> handler,
            CancellationToken cancellationToken,
            Mock<IModel> consumingChannelMock,
            [Frozen] Mock<IConnection> connectionMock,
            [Frozen] Mock<ISerializer> serializerMock,
            RabbitMQConsumingChannel consumingChannel)
        {
            // Arrange
            IBasicConsumer assignedConsumer = null;
            connectionMock
                .Setup(connection => connection.CreateModel())
                .Returns(consumingChannelMock.Object);

            consumingChannelMock
                .Setup(model => model.BasicConsume(
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<IDictionary<string, object>>(),
                    It.IsAny<IBasicConsumer>()))
                .Returns(consumingTag)
                .Callback<string, bool, string, bool, bool, IDictionary<string, object>, IBasicConsumer>(
                    (_, _, _, _, _, _, consumer) =>
                    {
                        assignedConsumer = consumer;
                    });
            
            consumingChannel.AddConsumer(handler, cancellationToken);

            var asyncEventingBasicConsumer = Assert.IsType<AsyncEventingBasicConsumer>(assignedConsumer);
            
            // Act
            await asyncEventingBasicConsumer.HandleBasicDeliver(
                consumingTag,
                deliveryTag,
                redelivered,
                exchange,
                routingKey,
                basicProperties,
                body);

            // Assert
            serializerMock.Verify(
                serializer =>
                    serializer.DeserializeFromUtf8Bytes(
                        It.Is<byte[]>(bytes => bytes.SequenceEqual(body.ToArray())),
                        typeof(object)), 
                Times.Once);
            
            serializerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task LogErrorAndDoNotThrow_When_HandlingMessage_And_DeserializingBodyFromUtf8BytesToEventStreamEntryThrows(
            string consumingTag,
            ulong deliveryTag,
            bool redelivered,
            string exchange,
            string routingKey,
            IBasicProperties basicProperties,
            ReadOnlyMemory<byte> body,
            Exception exception,
            Func<object, CancellationToken, Task> handler,
            CancellationToken cancellationToken,
            Mock<IModel> consumingChannelMock,
            [Frozen] Mock<IConnection> connectionMock,
            [Frozen] Mock<ISerializer> serializerMock,
            [Frozen] Mock<ILogger<RabbitMQConsumingChannel>> loggerMock,
            RabbitMQConsumingChannel consumingChannel)
        {
            // Arrange
            IBasicConsumer assignedConsumer = null;
            serializerMock
                .Setup(serializer => serializer.DeserializeFromUtf8Bytes(It.IsAny<byte[]>(), It.IsAny<Type>()))
                .Throws(exception);
            
            connectionMock
                .Setup(connection => connection.CreateModel())
                .Returns(consumingChannelMock.Object);

            consumingChannelMock
                .Setup(model => model.BasicConsume(
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<IDictionary<string, object>>(),
                    It.IsAny<IBasicConsumer>()))
                .Returns(consumingTag)
                .Callback<string, bool, string, bool, bool, IDictionary<string, object>, IBasicConsumer>(
                    (_, _, _, _, _, _, consumer) =>
                    {
                        assignedConsumer = consumer;
                    });
            
            consumingChannel.AddConsumer(handler, cancellationToken);

            var asyncEventingBasicConsumer = Assert.IsType<AsyncEventingBasicConsumer>(assignedConsumer);
            
            // Act & Assert
            await asyncEventingBasicConsumer.HandleBasicDeliver(
                consumingTag,
                deliveryTag,
                redelivered,
                exchange,
                routingKey,
                basicProperties,
                body);

            loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Error,
                    0,
                    It.IsAny<It.IsAnyType>(),
                    exception,
                    It.IsAny<Func<It.IsAnyType,Exception,string>>()),
                Times.Once);
        }

        [Theory]
        [AutoMoqData]
        internal async Task HandleDeserializedObject_When_HandlingMessage(
            string consumingTag,
            ulong deliveryTag,
            bool redelivered,
            string exchange,
            string routingKey,
            IBasicProperties basicProperties,
            ReadOnlyMemory<byte> body,
            object deserializedObject,
            Mock<Func<object, CancellationToken, Task>> handlerMock,
            CancellationToken cancellationToken,
            Mock<IModel> consumingChannelMock,
            [Frozen] Mock<IConnection> connectionMock,
            [Frozen] Mock<ISerializer> serializerMock,
            RabbitMQConsumingChannel consumingChannel)
        {
            // Arrange
            IBasicConsumer assignedConsumer = null;
            connectionMock
                .Setup(connection => connection.CreateModel())
                .Returns(consumingChannelMock.Object);

            consumingChannelMock
                .Setup(model => model.BasicConsume(
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<IDictionary<string, object>>(),
                    It.IsAny<IBasicConsumer>()))
                .Returns(consumingTag)
                .Callback<string, bool, string, bool, bool, IDictionary<string, object>, IBasicConsumer>(
                    (_, _, _, _, _, _, consumer) =>
                    {
                        assignedConsumer = consumer;
                    });
            
            consumingChannel.AddConsumer(handlerMock.Object, cancellationToken);

            var asyncEventingBasicConsumer = Assert.IsType<AsyncEventingBasicConsumer>(assignedConsumer);
            serializerMock
                .Setup(serializer => serializer.DeserializeFromUtf8Bytes(It.IsAny<byte[]>(), It.IsAny<Type>()))
                .Returns(deserializedObject);
            
            // Act
            await asyncEventingBasicConsumer.HandleBasicDeliver(
                consumingTag,
                deliveryTag,
                redelivered,
                exchange,
                routingKey,
                basicProperties,
                body);

            // Assert
            handlerMock.Verify(handler => handler(deserializedObject, cancellationToken), Times.Once);
            handlerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task LogErrorAndDoNotThrow_When_HandlingMessage_And_HandleDeserializedObject(
            string consumingTag,
            ulong deliveryTag,
            bool redelivered,
            string exchange,
            string routingKey,
            IBasicProperties basicProperties,
            ReadOnlyMemory<byte> body,
            Exception exception,
            Mock<Func<object, CancellationToken, Task>> handlerMock,
            CancellationToken cancellationToken,
            Mock<IModel> consumingChannelMock,
            [Frozen] Mock<IConnection> connectionMock,
            [Frozen] Mock<ILogger<RabbitMQConsumingChannel>> loggerMock,
            RabbitMQConsumingChannel consumingChannel)
        {
            // Arrange
            IBasicConsumer assignedConsumer = null;
            
            connectionMock
                .Setup(connection => connection.CreateModel())
                .Returns(consumingChannelMock.Object);

            consumingChannelMock
                .Setup(model => model.BasicConsume(
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<IDictionary<string, object>>(),
                    It.IsAny<IBasicConsumer>()))
                .Returns(consumingTag)
                .Callback<string, bool, string, bool, bool, IDictionary<string, object>, IBasicConsumer>(
                    (_, _, _, _, _, _, consumer) =>
                    {
                        assignedConsumer = consumer;
                    });
            
            consumingChannel.AddConsumer(handlerMock.Object, cancellationToken);

            var asyncEventingBasicConsumer = Assert.IsType<AsyncEventingBasicConsumer>(assignedConsumer);

            handlerMock
                .Setup(handler => handler(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .Throws(exception);
            
            // Act & Assert
            await asyncEventingBasicConsumer.HandleBasicDeliver(
                consumingTag,
                deliveryTag,
                redelivered,
                exchange,
                routingKey,
                basicProperties,
                body);

            loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Error,
                    0,
                    It.IsAny<It.IsAnyType>(),
                    exception,
                    It.IsAny<Func<It.IsAnyType,Exception,string>>()),
                Times.Once);
        }

        [Theory]
        [AutoMoqData]
        internal async Task LogErrorAndDoNotAcknowledgeMessage_When_HandlingMessage_And_CancellationIsRequested(
            string consumingTag,
            ulong deliveryTag,
            bool redelivered,
            string exchange,
            string routingKey,
            IBasicProperties basicProperties,
            ReadOnlyMemory<byte> body,
            Mock<Func<object, CancellationToken, Task>> handlerMock,
            CancellationTokenSource cancellationTokenSource,
            Mock<IModel> consumingChannelMock,
            [Frozen] Mock<IConnection> connectionMock,
            [Frozen] Mock<ILogger<RabbitMQConsumingChannel>> loggerMock,
            RabbitMQConsumingChannel consumingChannel)
        {
            // Arrange
            IBasicConsumer assignedConsumer = null;
            
            connectionMock
                .Setup(connection => connection.CreateModel())
                .Returns(consumingChannelMock.Object);

            consumingChannelMock
                .Setup(model => model.BasicConsume(
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<IDictionary<string, object>>(),
                    It.IsAny<IBasicConsumer>()))
                .Returns(consumingTag)
                .Callback<string, bool, string, bool, bool, IDictionary<string, object>, IBasicConsumer>(
                    (_, _, _, _, _, _, consumer) =>
                    {
                        assignedConsumer = consumer;
                    });
            
            consumingChannel.AddConsumer(handlerMock.Object, cancellationTokenSource.Token);

            var asyncEventingBasicConsumer = Assert.IsType<AsyncEventingBasicConsumer>(assignedConsumer);
            
            // Act & Assert
            cancellationTokenSource.Cancel();
            await asyncEventingBasicConsumer.HandleBasicDeliver(
                consumingTag,
                deliveryTag,
                redelivered,
                exchange,
                routingKey,
                basicProperties,
                body);

            loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Error,
                    0,
                    It.IsAny<It.IsAnyType>(),
                    null,
                    It.IsAny<Func<It.IsAnyType,Exception,string>>()),
                Times.Once);
            
            consumingChannelMock.Verify(model => model.BasicAck(It.IsAny<ulong>(), It.IsAny<bool>()), Times.Never);
        }

        [Theory]
        [AutoMoqData]
        internal async Task AcknowledgeMessage_When_HandlingMessage_And_CancellationIsNotRequested(
            string consumingTag,
            ulong deliveryTag,
            bool redelivered,
            string exchange,
            string routingKey,
            IBasicProperties basicProperties,
            ReadOnlyMemory<byte> body,
            Mock<Func<object, CancellationToken, Task>> handlerMock,
            Mock<IModel> consumingChannelMock,
            [Frozen] Mock<IConnection> connectionMock,
            RabbitMQConsumingChannel consumingChannel)
        {
            // Arrange
            IBasicConsumer assignedConsumer = null;
            
            connectionMock
                .Setup(connection => connection.CreateModel())
                .Returns(consumingChannelMock.Object);

            consumingChannelMock
                .Setup(model => model.BasicConsume(
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<IDictionary<string, object>>(),
                    It.IsAny<IBasicConsumer>()))
                .Returns(consumingTag)
                .Callback<string, bool, string, bool, bool, IDictionary<string, object>, IBasicConsumer>(
                    (_, _, _, _, _, _, consumer) =>
                    {
                        assignedConsumer = consumer;
                    });
            
            consumingChannel.AddConsumer(handlerMock.Object, CancellationToken.None);

            var asyncEventingBasicConsumer = Assert.IsType<AsyncEventingBasicConsumer>(assignedConsumer);
            
            // Act & Assert
            await asyncEventingBasicConsumer.HandleBasicDeliver(
                consumingTag,
                deliveryTag,
                redelivered,
                exchange,
                routingKey,
                basicProperties,
                body);
            
            consumingChannelMock.Verify(model => model.BasicAck(deliveryTag, false), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        internal async Task LogErrorAndDoNotThrow_When_HandlingMessage_And_AcknowledgingThrowsException(
            string consumingTag,
            ulong deliveryTag,
            bool redelivered,
            string exchange,
            string routingKey,
            IBasicProperties basicProperties,
            ReadOnlyMemory<byte> body,
            Exception exception,
            Mock<Func<object, CancellationToken, Task>> handlerMock,
            Mock<IModel> consumingChannelMock,
            [Frozen] Mock<IConnection> connectionMock,
            [Frozen] Mock<ILogger<RabbitMQConsumingChannel>> loggerMock,
            RabbitMQConsumingChannel consumingChannel)
        {
            // Arrange
            IBasicConsumer assignedConsumer = null;
            
            connectionMock
                .Setup(connection => connection.CreateModel())
                .Returns(consumingChannelMock.Object);

            consumingChannelMock
                .Setup(model => model.BasicConsume(
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<IDictionary<string, object>>(),
                    It.IsAny<IBasicConsumer>()))
                .Returns(consumingTag)
                .Callback<string, bool, string, bool, bool, IDictionary<string, object>, IBasicConsumer>(
                    (_, _, _, _, _, _, consumer) =>
                    {
                        assignedConsumer = consumer;
                    });
            
            consumingChannel.AddConsumer(handlerMock.Object, CancellationToken.None);

            var asyncEventingBasicConsumer = Assert.IsType<AsyncEventingBasicConsumer>(assignedConsumer);
            consumingChannelMock
                .Setup(model => model.BasicAck(It.IsAny<ulong>(), It.IsAny<bool>()))
                .Throws(exception)
                .Verifiable();
            
            // Act & Assert
            await asyncEventingBasicConsumer.HandleBasicDeliver(
                consumingTag,
                deliveryTag,
                redelivered,
                exchange,
                routingKey,
                basicProperties,
                body);

            loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Error,
                    0,
                    It.IsAny<It.IsAnyType>(),
                    exception,
                    It.IsAny<Func<It.IsAnyType,Exception,string>>()),
                Times.Once);
            
            consumingChannelMock.Verify();
        }
    }
}