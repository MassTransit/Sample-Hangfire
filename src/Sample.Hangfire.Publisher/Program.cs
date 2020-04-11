using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Context;
using MassTransit.Definition;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;

namespace Sample.Hangfire.Publisher
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            const string QueueName = "hangfire";
            const string Rmq = "rabbitmq://guest:guest@localhost:5672";

            var logger = new LoggerConfiguration()
                .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information)
                .CreateLogger();
            var loggerFactory = new SerilogLoggerFactory(logger, true);

            var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                LogContext.ConfigureCurrentLogContext(loggerFactory);

                var host = cfg.Host(new Uri(Rmq));

                cfg.ReceiveEndpoint(KebabCaseEndpointNameFormatter.Instance.Consumer<MessageConsumer>(), configure =>
                {
                    configure.PrefetchCount = 16;
                    configure.Consumer(() => new MessageConsumer(loggerFactory.CreateLogger<MessageConsumer>()));
                });
                cfg.UseMessageScheduler(new UriBuilder(host.Address) {Path = QueueName}.Uri);
            });

            await bus.StartAsync();

            Console.WriteLine("Please enter a message and press Enter key, to quit 'q' and Enter key:");
            var message = Console.ReadLine();

            var uri = new UriBuilder(bus.Address) {Path = KebabCaseEndpointNameFormatter.Instance.Consumer<MessageConsumer>()}.Uri;
            while (message != "q")
            {
                await bus.ScheduleSend<IMessage>(uri, DateTime.Now.AddSeconds(10), new
                {
                    Id = NewId.NextGuid(),
                    Message = message
                });

                message = Console.ReadLine();
                Console.WriteLine("Please enter a message and press Enter key, to quit 'q' and Enter key:");
            }

            await bus.StopAsync();
            loggerFactory.Dispose();
        }
    }
}
