using System.Threading.Tasks;
using MassTransit;
using MassTransit.Context;
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
            var logger = new LoggerConfiguration()
                .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information)
                .CreateLogger();
            var loggerFactory = new SerilogLoggerFactory(logger, true);

            var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                LogContext.ConfigureCurrentLogContext(loggerFactory);

                cfg.Host(AppConfiguration.RmqUri);

                cfg.UseHangfireScheduler(AppConfiguration.HangfireQueueName, server => server.ServerName = "MT-Console");
            });

            await bus.StartAsync();

            System.Console.WriteLine("Press Enter to quit.");
            System.Console.ReadLine();

            await bus.StopAsync();

            loggerFactory.Dispose();
        }
    }
}
