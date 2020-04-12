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

            const string QueueName = "hangfire";
            const string ServerName = "MT-AspNetCore";
            const string Rmq = "rabbitmq://guest:guest@localhost:5672";

            static IBusControl CreateBus(IServiceProvider serviceProvider) => Bus.Factory.CreateUsingRabbitMq(configure =>
            {
                configure.Host(new Uri(Rmq));
                configure.UseHangfireScheduler(new ServiceProviderHangfireComponentResolver(serviceProvider), QueueName, server =>
                {
                    server.ServerName = ServerName;
                    server.Activator = serviceProvider.GetRequiredService<JobActivator>();
                });

                configure.UseHealthCheck(serviceProvider);
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
