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
            // TODO Prio 97: LicenseExpirationWarningBackgroundService hinzufügen: Sofort und täglich um Mitternacht überprüfen und bei X Tagen vor Ablauf warnen
            // Category=License	Key=expirationDate
        }
    }
}
