using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace AMTools.Web.BackgroundServices
{
    public static class BackgroundServiceInjector
    {
        public static void InjectBackgroundServices(this IServiceCollection services)
        {
            services.AddHostedService<SettingsBackgroundService>();
            services.AddHostedService<AvailabilityStatusBackgroundService>();
            services.AddHostedService<SubscriberBackgroundService>();
            services.AddHostedService<CalloutBackgroundService>();
        }
    }
}
