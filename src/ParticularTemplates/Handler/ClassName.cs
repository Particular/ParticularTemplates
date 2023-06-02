#if (!ImplicitUsings)
using System;
using System.Threading.Tasks;
#endif
using Microsoft.Extensions.Logging;
#if (!ImplicitUsings)
using NServiceBus;
#endif

namespace NamespaceName
{
    public class ClassName : IHandleMessages<MessageType>
    {
        private readonly ILogger log;

        public ClassName(ILogger<ClassName> log)
        {
            this.log = log;
        }

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
