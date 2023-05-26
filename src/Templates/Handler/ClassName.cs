using System;
using Microsoft.Extensions.Logging;
using NServiceBus;

namespace NamespaceName
{
    public class ClassName : IHandleMessages<MessageType>
    {
        private readonly ILog log;

        public ClassName(ILogger<ClassName> log)
        {
            this.log = log;
        }

        public async Task Handle(MessageType message, IMessageHandlerContext context)
        {

        }
    }
}
