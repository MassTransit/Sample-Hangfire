using System.Threading.Tasks;
using Hangfire;
using Hangfire.MemoryStorage;
using MassTransit;
using MassTransit.Context;
using Microsoft.Extensions.Logging;
using Sample.Hangfire.Core;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;

namespace Sample.Hangfire.Console
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            GlobalConfiguration.Configuration
                .UseMemoryStorage();

            var logger = new LoggerConfiguration()
                .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information)
                .CreateLogger();
            ILoggerFactory loggerFactory = new SerilogLoggerFactory(logger, true);

            var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                LogContext.ConfigureCurrentLogContext(loggerFactory);

                cfg.Host(AppConfiguration.RmqUri);

                cfg.UseHangfireScheduler(AppConfiguration.HangfireQueueName);
            });

            await bus.StartAsync();

            System.Console.WriteLine("Press Enter to quit.");
            System.Console.ReadLine();

            await bus.StopAsync();

            loggerFactory.Dispose();
        }
    }
}
