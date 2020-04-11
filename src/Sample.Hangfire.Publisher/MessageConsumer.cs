using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Util;
using Microsoft.Extensions.Logging;

namespace Sample.Hangfire.Publisher
{
    public class MessageConsumer : IConsumer<IMessage>
    {
        private readonly ILogger<MessageConsumer> _logger;

        public MessageConsumer(ILogger<MessageConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<IMessage> context)
        {
            _logger.LogInformation("Message: {message} ({id}) received at {receivedAt}",
                context.Message.Message,
                context.Message.Id,
                DateTime.Now);
            return TaskUtil.Completed;
        }
    }
}
