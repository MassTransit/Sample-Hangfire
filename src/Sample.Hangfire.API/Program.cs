using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace Sample.Hangfire.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder
                    .UseStartup<Startup>()
                    .UseShutdownTimeout(TimeSpan.FromSeconds(15)))
                .ConfigureLogging(context =>
                {
                    context.ClearProviders();

                    var logger = new LoggerConfiguration()
                        .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information)
                        .CreateLogger();
                    context.AddSerilog(logger, true);
                });
    }
}
