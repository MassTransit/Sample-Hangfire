using System;

namespace Sample.Hangfire.Core
{
    public class AppConfiguration
    {
        private const string RmqConnectionString = "rabbitmq://guest:guest@localhost:5672";
        public const string HangfireQueueName = "hangfire";

        public static Uri RmqUri => new Uri(RmqConnectionString);
    }
}
