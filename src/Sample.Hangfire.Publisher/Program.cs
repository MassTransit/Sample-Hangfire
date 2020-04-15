using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Context;
using MassTransit.Definition;
using Microsoft.Extensions.Logging;
using Sample.Hangfire.Core;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;

namespace Sample.Hangfire.Publisher
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var logger = new LoggerConfiguration()
                .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information)
                .CreateLogger();
            var loggerFactory = new SerilogLoggerFactory(logger, true);

            var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                LogContext.ConfigureCurrentLogContext(loggerFactory);

                var host = cfg.Host(AppConfiguration.RmqUri);

                cfg.ReceiveEndpoint(KebabCaseEndpointNameFormatter.Instance.Consumer<MessageConsumer>(), configure =>
                {
                    configure.PrefetchCount = 16;
                    configure.Consumer(() => new MessageConsumer(loggerFactory.CreateLogger<MessageConsumer>()));
                });
                cfg.UseMessageScheduler(new Uri($"queue:{AppConfiguration.HangfireQueueName}"));
            });

            await bus.StartAsync();

            Console.WriteLine("Please enter a message and press Enter key, to quit 'q' and Enter key:");
            var message = Console.ReadLine();

            while (message != "q")
            {
                await bus.ScheduleSend<IMessage>(new Uri($"queue:{KebabCaseEndpointNameFormatter.Instance.Consumer<MessageConsumer>()}"), DateTime.Now.AddSeconds(10), new
                {
                    Id = NewId.NextGuid(),
                    Message = message
                });

                Console.WriteLine("Please enter a message and press Enter key, to quit 'q' and Enter key:");
                message = Console.ReadLine();
            }

            await bus.StopAsync();
            loggerFactory.Dispose();
        }
    }
}
