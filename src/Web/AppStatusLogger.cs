using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Core.Services.Logging;
using Microsoft.Extensions.Hosting;

namespace AMTools.Web
{
    public static class AppStatusLogger
    {
        public static void LogAppStatusChanges(this IHostApplicationLifetime hostApplicationLifetime, IServiceProvider serviceProvider)
        {
            var logService = serviceProvider.GetService(typeof(ILogService)) as ILogService;
            hostApplicationLifetime?.ApplicationStarted.Register(() =>
            {
                logService?.Info("Die Anwendung wurde gestartet!");
            });

            hostApplicationLifetime?.ApplicationStopping.Register(() =>
            {
                logService?.Info("Die Anwendung wird angehalten!");
            });

            hostApplicationLifetime?.ApplicationStopped.Register(() =>
            {
                logService?.Info("Die Anwendung wurde angehalten!");
            });
        }
    }
}
