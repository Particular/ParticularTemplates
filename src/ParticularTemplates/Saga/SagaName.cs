using Microsoft.Extensions.Logging;

namespace NamespaceName;

internal class SagaName(ILogger<SagaName> log) : Saga<SagaNameData>, IAmStartedByMessages<MessageType1>, IAmStartedByMessages<MessageType2>, IHandleTimeouts<MyCustomTimeout>
{
    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<SagaNameData> mapper)
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

    public async Task Timeout(MyCustomTimeout timeout, IMessageHandlerContext context)
    {
        // Remove if saga does not require timeouts
    }
}

internal class SagaNameData : ContainSagaData
{
    public string CorrelationId { get; set; }
    // Other properties
}

internal class MyCustomTimeout
{
    // Optional extra properties
}
