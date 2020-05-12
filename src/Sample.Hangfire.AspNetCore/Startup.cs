using System;
using Hangfire;
using Hangfire.MemoryStorage;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sample.Hangfire.Core;

namespace Sample.Hangfire.AspNetCore
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddHealthChecks();

            static IBusControl CreateBus(IRegistrationContext<IServiceProvider> context) => Bus.Factory.CreateUsingRabbitMq(configure =>
            {
                configure.Host(AppConfiguration.RmqUri);
                configure.UseHangfireScheduler(new ServiceProviderHangfireComponentResolver(context.Container), AppConfiguration.HangfireQueueName, server =>
                {
                    server.ServerName = "MT-AspNetCore";
                    server.Activator = context.Container.GetRequiredService<JobActivator>();
                });

                configure.UseHealthCheck(context);
            });

            services
                .AddHangfire(configure => configure.UseMemoryStorage())
                .AddMassTransit(configure => configure.AddBus(CreateBus))
                .AddMassTransitHostedService();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection()
                .UseRouting()
                .UseHangfireDashboard()
                .UseHealthChecks("/health");
        }
    }
}
