namespace Sample.Hangfire.AspNetCore
{
    using global::Hangfire;
    using global::Hangfire.MemoryStorage;
    using MassTransit;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;


    public class Startup
    {

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddHangfireServer();

            services
                .AddHangfire(x => x.UseMemoryStorage())
                .AddMassTransit(x =>
                {
                    x.UsingRabbitMq((_, cfg) =>
                    {
                        cfg.Host("rabbitmq://rabbit-server/host", configurator =>
                        {
                            configurator.Username("user");
                            configurator.Password("password");
                        });
                    });


                    x.AddHangfireConsumers();
                });
        }

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
