namespace Sample.Hangfire.Console
{
    using System.Threading.Tasks;
    using global::Hangfire;
    using global::Hangfire.MemoryStorage;
    using MassTransit;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Serilog;
    using Serilog.Events;


    public static class Program
    {
        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("MassTransit", LogEventLevel.Debug)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            var host = CreateHostBuilder(args).Build();

            await host.RunAsync();
        }

        static IHostBuilder CreateHostBuilder(string[] args)
        {
            GlobalConfiguration.Configuration
                .UseMemoryStorage();

            return Host.CreateDefaultBuilder(args)
                .ConfigureHostConfiguration(config => config.AddEnvironmentVariables())
                .UseSerilog()
                .ConfigureServices((_, services) =>
                {
                    services.AddMassTransit(x =>
                    {
                        x.AddPublishMessageScheduler();

                        x.UsingRabbitMq((context, cfg) =>
                        {
                            cfg.Host("rabbitmq://rabbit-server/host", configurator =>
                            {
                                configurator.Username("user");
                                configurator.Password("password");
                            });

                            cfg.UsePublishMessageScheduler();

                            cfg.UseHangfireScheduler();

                            cfg.ConfigureEndpoints(context);
                        });
                    });

                    services.AddOptions<MassTransitHostOptions>().Configure(options =>
                    {
                        options.WaitUntilStarted = true;
                    });
                });
        }
    }
}
