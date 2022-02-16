using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EventSourcing.Bus.RabbitMQ.Transport
{
    internal class RabbitMQChannel : IRabbitMQChannel, IDisposable
    {
        private readonly IModel _model;
        private readonly ILogger<RabbitMQChannel> _logger;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly Guid _channelId = Guid.NewGuid();

        public RabbitMQChannel(
            IModel model,
            ILogger<RabbitMQChannel> logger)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task ExecuteActionInThreadSafeMannerAsync(Action<IModel> action, CancellationToken cancellationToken)
        {
            var semaphoreStopwatch = Stopwatch.StartNew();
            await _semaphore.WaitAsync(cancellationToken);
            try
            {
                semaphoreStopwatch.Stop();
                _logger.LogDebug(
                    "Waiting asynchronously to acquire semaphore for model action took {LockingTime}. Channel: {RabbitMQChannel}",
                    semaphoreStopwatch.Elapsed,
                    this);
                
                action(_model);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public void AttachEventLoggingHandlers()
        {
            _model.BasicAcks += ModelOnBasicAcks;
            _model.BasicNacks += ModelOnBasicNacks;
            _model.BasicReturn += ModelOnBasicReturn;
            _model.CallbackException += ModelOnCallbackException;
            _model.FlowControl += ModelOnFlowControl;
            _model.ModelShutdown += ModelOnModelShutdown;
            _model.BasicRecoverOk += ModelOnBasicRecoverOk;
        }

        public void Dispose()
        {
            _model?.Dispose();
        }

        public override string ToString()
        {
            return $"RabbitMQ Channel with id {_channelId}. Model: {_model}";
        }

        private void ModelOnBasicRecoverOk(object sender, EventArgs e)
        {
            _logger.LogInformation(
                "Model basic recover ok. Sender: {ModelBasicRecoverOkSender}, EventArgs: {@EventArgs}, Channel: {RabbitMQChannel}",
                sender,
                e,
                this);
        }

        private void ModelOnModelShutdown(object sender, ShutdownEventArgs e)
        {
            _logger.LogInformation(
                "Model shutdown. Sender: {ModelShutdownSender}, EventArgs: {ShutdownEventArgs}, Channel: {RabbitMQChannel}",
                sender,
                e,
                this);
        }

        private void ModelOnFlowControl(object sender, FlowControlEventArgs e)
        {
            _logger.LogInformation(
                "Model on flow control. Sender: {ModelOnFlowControlSender}, EventArgs: {@FlowControlEventArgs}, Channel: {RabbitMQChannel}",
                sender,
                e,
                this);
        }

        private void ModelOnCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            _logger.LogWarning(
                e.Exception,
                "Model callback exception. Sender: {ModelCallbackExceptionSender}, EventArgs: {CallbackExceptionEventArgs}, Detail: {@CallbackExceptionDetail}, Channel: {RabbitMQChannel}",
                sender,
                e,
                e.Detail,
                this);
        }

        private void ModelOnBasicReturn(object sender, BasicReturnEventArgs e)
        {
            _logger.LogInformation(
                "Model on basic return. Sender: {ModelOnBasicReturnSender}, EventArgs: {@SerializableBasicReturnEventArgs}, Channel: {RabbitMQChannel}",
                sender,
                SerializableBasicReturnEventArgs.FromBasicReturnEventArgs(e),
                this);
        }

        private void ModelOnBasicNacks(object sender, BasicNackEventArgs e)
        {
            _logger.LogDebug(
                "Model on basic nacks. Sender: {ModelOnBasicNacksSender}, EventArgs: {@BasicNackEventArgs}, Channel: {RabbitMQChannel}",
                sender,
                e,
                this);
        }

        private void ModelOnBasicAcks(object sender, BasicAckEventArgs e)
        {
            _logger.LogDebug(
                "Model on basic acks. Sender: {ModelOnBasicAcksSender}, EventArgs: {@BasicAckEventArgs}, Channel: {RabbitMQChannel}",
                sender,
                e,
                this);
        }

        private class SerializableBasicReturnEventArgs
        {
            public static SerializableBasicReturnEventArgs FromBasicReturnEventArgs(BasicReturnEventArgs args) =>
                new SerializableBasicReturnEventArgs
                {
                    BasicProperties = args.BasicProperties,
                    Exchange = args.Exchange,
                    ReplyCode = args.ReplyCode,
                    ReplyText = args.ReplyText,
                    RoutingKey = args.RoutingKey
                };

            public IBasicProperties BasicProperties { get; set; }

            public string Exchange { get; set; }

            public ushort ReplyCode { get; set; }

            public string ReplyText { get; set; }

            public string RoutingKey { get; set; }
        }
    }
}