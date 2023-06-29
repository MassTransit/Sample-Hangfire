namespace Sample.Hangfire.Publisher
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Calabonga.Microservices.BackgroundWorkers;
    using MassTransit;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;


    public class Worker : ScheduledHostedServiceBase
    {
        readonly IMessageScheduler _scheduler;
        bool _includingSeconds = true;

        public Worker(IServiceScopeFactory serviceProvider, IMessageScheduler scheduler, ILogger<Worker> logger)
            : base(serviceProvider, logger)
        {
            _scheduler = scheduler;
        }

        protected override string Schedule => "*/10 * * * * *";

        protected override string DisplayName => "Schedule publisher";

        protected override async Task ProcessInScopeAsync(IServiceProvider serviceProvider, CancellationToken token)
        {
            await _scheduler.SchedulePublish<IMessage>(DateTime.UtcNow.AddSeconds(10), new
            {
                InVar.Id,
                Message = $"Hello {InVar.Timestamp}"
            }, token);
        }

        protected override bool IncludingSeconds
        {
            get => _includingSeconds;
            set => _includingSeconds = value;
        }

        protected override bool IsDelayBeforeStart => true;

        protected override bool IsExecuteOnServerRestart => true;
    }
}
