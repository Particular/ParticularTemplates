using Microsoft.Extensions.Logging;

namespace NamespaceName
{
    internal class ClassName(ILogger<ClassName> log) : IHandleMessages<MessageType>
    {
        public async Task Handle(MessageType message, IMessageHandlerContext context)
        {
            // Business logic here

            // Sending commands: https://docs.particular.net/nservicebus/messaging/send-a-message#inside-the-incoming-message-processing-pipeline
            // await context.Send(...);

            // Publishing events https://docs.particular.net/nservicebus/messaging/publish-subscribe/publish-handle-event
            // await context.Publish(...);
        }
    }
}
