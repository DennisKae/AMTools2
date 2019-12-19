using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AMTools.Core.Services.Logging;
using AMTools.Shared.Core.Repositories;
using AMTools.Shared.Core.Repositories.Interfaces;
using AMTools.Shared.Core.Services;
using AMTools.Shared.Core.Services.Interfaces;
using AMTools.Shared.Core.Services.Logging;
using AMTools.Shared.Core.Services.VirtualDesktops;
using AMTools.Shared.Core.Services.VirtualDesktops.Interfaces;
using AMTools.Web.BackgroundServices;
using AMTools.Web.Core.Facades;
using AMTools.Web.Core.Facades.Interfaces;
using AMTools.Web.Core.Services;
using AMTools.Web.Core.Services.DataSynchronization;
using AMTools.Web.Core.Services.DataSynchronization.Interfaces;
using AMTools.Web.Core.Services.Interfaces;
using AMTools.Web.Core.Services.Settings;
using AMTools.Web.Core.Services.Settings.Interfaces;
using AMTools.Web.Data.Database;
using AMTools.Web.Data.Files.Repositories;
using AMTools.Web.Data.Files.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace AMTools.Web
{
    public static class DependencyInjector
    {
        /// <summary>Injiziert alle benötigten Dependencies</summary>
        public static void InjectDependencies(this IServiceCollection services)
        {
            // Singleton means only a single instance will ever be created. That instance is shared between all components that require it. The same instance is thus used always.
            // Scoped means an instance is created once per scope. A scope is created on every request to the application, thus any components registered as Scoped will be created once per request.
            // Transient components are created every time they are requested and are never shared.

            ILogFactory logFactory = GetLogFactory();

            try
            {
                services.AddDbContext<DatabaseContext>();
                services.AddSingleton<IConfigurationFileRepository, ConfigurationFileRepository>();

                services.AddSingleton(logFactory);
                services.AddSingleton<ILogService>(serviceProvider =>
                {
                    ILogFactory logFactory = serviceProvider.GetService<ILogFactory>();
                    return logFactory;
                });

                // FileRepositories
                services.AddSingleton<IAvailabilityFileRepository, AvailabilityFileRepository>();
                services.AddSingleton<ICalloutFileRepository, CalloutFileRepository>();
                services.AddSingleton<ISettingsFileRepository, SettingsFileRepository>();
                services.AddSingleton<ISubscriberFileRepository, SubscriberFileRepository>();

                // SyncServices
                services.AddSingleton<IAlertSyncService, AlertSyncService>();
                services.AddSingleton<IAvailabilityStatusSyncService, AvailabilityStatusSyncService>();
                services.AddSingleton<ISettingsSyncService, SettingsSyncService>();
                services.AddSingleton<ISubscriberSyncService, SubscriberSyncService>();
                services.AddSingleton<IUserResponseSyncService, UserResponseSyncService>();

                // BackgroundServices
                services.AddHostedService<SettingsBackgroundService>();
                services.AddHostedService<AvailabilityStatusBackgroundService>();
                services.AddHostedService<SubscriberBackgroundService>();
                services.AddHostedService<CalloutBackgroundService>();

                // Setting Services
                services.AddSingleton<IQualificationService, QualificationService>();
                services.AddSingleton<ISettingsService, SettingsService>();
                services.AddSingleton<ISeverityLevelService, SeverityLevelService>();
                services.AddSingleton<ISubGroupService, SubGroupService>();

                // Andere Services
                services.AddSingleton<ITerminalService, TerminalService>();
                services.AddSingleton<IVirtualDesktopVersionService, VirtualDesktopVersionService>();
                services.AddSingleton<IVirtualDesktopWrapperService, VirtualDesktopWrapperService>();
                services.AddSingleton<IVirtualDesktopService, VirtualDesktopService>();
                services.AddSingleton<IAlertService, AlertService>();
                services.AddSingleton<ICalloutNotificationService, CalloutNotificationService>();
                services.AddSingleton<ISubscriberService, SubscriberService>();
                services.AddSingleton<IUserResponseService, UserResponseService>();

                // Facades
                services.AddSingleton<IVirtualDesktopFacade, VirtualDesktopFacade>();
            }
            catch (Exception exception)
            {
                logFactory.Exception(exception, "Exception bei der Dependency Injection.");
            }
        }

        private static ILogFactory GetLogFactory()
        {
            string assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            var consoleLogService = new ConsoleLogService(assemblyName, null);
            var dbLogService = new DbLogService(assemblyName, null, consoleLogService);

            var result = new LogFactory(consoleLogService, assemblyName, null);
            result.LoggingServices.Add(dbLogService);
            return result;
        }
    }
}
