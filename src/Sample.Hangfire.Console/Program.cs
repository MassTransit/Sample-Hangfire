using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Context;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;

namespace Sample.Hangfire.Console
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            const string QueueName = "hangfire";
            const string ServerName = "MT-Console";
            const string Rmq = "rabbitmq://guest:guest@localhost:5672";

            var logger = new LoggerConfiguration()
                .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information)
                .CreateLogger();
            var loggerFactory = new SerilogLoggerFactory(logger, true);

            var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                LogContext.ConfigureCurrentLogContext(loggerFactory);

                cfg.Host(new Uri(Rmq));

                cfg.UseHangfireScheduler(QueueName, server => server.ServerName = ServerName);
            });

            await bus.StartAsync();

            System.Console.WriteLine("Press Enter to quit.");
            System.Console.ReadLine();

            await bus.StopAsync();

            loggerFactory.Dispose();
        }
    }
}
