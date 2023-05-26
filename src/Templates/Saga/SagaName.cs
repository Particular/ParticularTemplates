using System;
using Microsoft.Extensions.Logging;
using NServiceBus;

namespace NamespaceName
{
    public class SagaName : Saga<SagaNameData>,
        IAmStartedByMessages<MessageType1>,
        IAmStartedByMessages<MessageType2>,
        IHandleTimeouts<MyCustomTimeout>
    {
        private readonly ILog log;

        public SagaName(ILogger<ClassName> log)
        {
            this.log = log;
        }

        protected override ConfigureHowToFindSaga(SagaPropertyMapper<SagaNameData> mapper)
        {
            // https://docs.particular.net/nservicebus/sagas/message-correlation
            mapper.MapSaga(saga => saga.CorrelationId)
                .ToMessage<MessageType1>(message => message.CorrelationId)
                .ToMessage<MessageType2>(message => message.CorrelationId);
        }

        public async Task Handle(MessageType1 message, IMessageHandlerContext context)
        {
            // Business logic here
        }

        public async Task Timeout(TimeoutType timeout, IMessageHandlerContext context)
        {
            // Remove if saga does not require timeouts
        }

        public async Task Handle(MessageType2 message, IMessageHandlerContext context)
        {
            // Update saga data: https://docs.particular.net/nservicebus/sagas/#long-running-means-stateful
            // this.Data.Property = ...

            // Sending commands: https://docs.particular.net/nservicebus/messaging/send-a-message#inside-the-incoming-message-processing-pipeline
            // await context.Send(...);

            // Publishing events https://docs.particular.net/nservicebus/messaging/publish-subscribe/publish-handle-event
            // await context.Publish(...);

            // Request a timeout: https://docs.particular.net/nservicebus/sagas/timeouts
            // await RequestTimeout<MyCustomTimeout>(context, TimeSpan.FromMinutes(10));

            // Ending a saga: https://docs.particular.net/nservicebus/sagas/#ending-a-saga
            // MarkAsComplete();
        }
    }

    class SagaNameData : ContainSagaData
    {
        public string CorrelationId { get; set; }
        // Other properties
    }

    class MyCustomTimeout
    {
        // Optional extra properties
    }
}
