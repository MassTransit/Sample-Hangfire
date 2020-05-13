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

            static IBusControl CreateBus(IRegistrationContext<IServiceProvider> context) => Bus.Factory.CreateUsingRabbitMq(configurator =>
            {
                configurator.Host(AppConfiguration.RmqUri);
                configurator.UseHangfireScheduler(context);
                configurator.UseHealthCheck(context);
            });

            services
                .AddHangfire(configuration => configuration.UseMemoryStorage())
                .AddMassTransit(configurator => configurator.AddBus(CreateBus))
                .AddMassTransitHostedService();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection()
                .UseRouting()
                .UseHangfireDashboard()
                .UseHealthChecks("/health");
        }
    }
}
