namespace Sample.Hangfire.Publisher
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit;
    using Microsoft.Extensions.Hosting;


    public class Worker :
        BackgroundService
    {
        readonly IMessageScheduler _scheduler;

        public Worker(IMessageScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _scheduler.SchedulePublish<IMessage>(DateTime.UtcNow.AddSeconds(10), new
            {
                InVar.Id,
                Message = "Hello"
            }, stoppingToken);
        }
    }
}
