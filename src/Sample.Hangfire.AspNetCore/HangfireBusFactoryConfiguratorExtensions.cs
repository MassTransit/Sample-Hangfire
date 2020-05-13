using System;
using System.Collections.Generic;
using Hangfire;
using Hangfire.Common;
using Hangfire.Server;
using MassTransit;
using MassTransit.HangfireIntegration;
using Microsoft.Extensions.DependencyInjection;
using Sample.Hangfire.Core;

namespace Sample.Hangfire.AspNetCore
{
    public static class HangfireBusFactoryConfiguratorExtensions
    {
        public static void UseHangfireScheduler(this IBusFactoryConfigurator configurator, IRegistrationContext<IServiceProvider> context)
        {
            configurator.UseHangfireScheduler(new ServiceProviderHangfireComponentResolver(context.Container), AppConfiguration.HangfireQueueName);
        }

        private class ServiceProviderHangfireComponentResolver :
            IHangfireComponentResolver
        {
            private readonly IServiceProvider _serviceProvider;

            public ServiceProviderHangfireComponentResolver(IServiceProvider serviceProvider)
            {
                _serviceProvider = serviceProvider;
            }

            public IBackgroundJobClient BackgroundJobClient => _serviceProvider.GetService<IBackgroundJobClient>();
            public IRecurringJobManager RecurringJobManager => _serviceProvider.GetService<IRecurringJobManager>();
            public ITimeZoneResolver TimeZoneResolver => _serviceProvider.GetService<ITimeZoneResolver>();
            public IJobFilterProvider JobFilterProvider => _serviceProvider.GetService<IJobFilterProvider>();
            public IEnumerable<IBackgroundProcess> BackgroundProcesses => _serviceProvider.GetServices<IBackgroundProcess>();
            public JobStorage JobStorage => _serviceProvider.GetService<JobStorage>();
        }
    }
}
