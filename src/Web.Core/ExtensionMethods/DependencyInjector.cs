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
using AMTools.Web.Core.Facades;
using AMTools.Web.Core.Facades.Interfaces;
using AMTools.Web.Core.Services;
using AMTools.Web.Core.Services.DataSynchronization;
using AMTools.Web.Core.Services.DataSynchronization.Interfaces;
using AMTools.Web.Core.Services.Interfaces;
using AMTools.Web.Core.Services.Settings;
using AMTools.Web.Core.Services.Settings.Interfaces;
using AMTools.Web.Core.Services.VirtualDesktops;
using AMTools.Web.Core.Services.VirtualDesktops.Interfaces;
using AMTools.Web.Data.Database;
using AMTools.Web.Data.Files.Repositories;
using AMTools.Web.Data.Files.Repositories.Interfaces;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace AMTools.Web.Core.ExtensionMethods
{
    public static class DependencyInjector
    {
        /// <summary>Injiziert alle benötigten Dependencies</summary>
        public static void InjectDependencies(this IServiceCollection services, string appPart) => InjectDependencies(services, appPart, null);

        /// <summary>Injiziert alle benötigten Dependencies</summary>
        public static void InjectDependencies(this IServiceCollection services, string appPart, string batchCommand)
        {
            // Singleton means only a single instance will ever be created. That instance is shared between all components that require it. The same instance is thus used always.
            // Scoped means an instance is created once per scope. A scope is created on every request to the application, thus any components registered as Scoped will be created once per request.
            // Transient components are created every time they are requested and are never shared.

            var configurationFileRepository = new ConfigurationFileRepository();
            ILogFactory logFactory = GetLogFactory(configurationFileRepository, appPart, batchCommand);

            try
            {
                services.AddAutoMapper(GetOwnAssemblies());

                services.AddSingleton<IConfigurationFileRepository>(serviceProvider => configurationFileRepository);

                services.AddSingleton(logFactory);
                services.AddSingleton<ILogService>(serviceProvider =>
                {
                    ILogFactory retrievedLogFactory = serviceProvider.GetService<ILogFactory>();
                    return retrievedLogFactory;
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

                // Setting Services
                services.AddSingleton<IQualificationSettingService, QualificationSettingService>();
                services.AddSingleton<ISettingsService, SettingsService>();
                services.AddSingleton<ISeverityLevelSettingService, SeverityLevelSettingService>();
                services.AddSingleton<ISubGroupSettingService, SubGroupSettingService>();
                services.AddSingleton<IGuiVisibilitySettingService, GuiVisibilitySettingService>();
                services.AddSingleton<IAvailabilityStatusSettingService, AvailabilityStatusSettingService>();

                // Andere Services
                services.AddSingleton<ITerminalService, TerminalService>();
                services.AddSingleton<IEmailService, EmailService>();
                services.AddSingleton<IVirtualDesktopVersionService, VirtualDesktopVersionService>();
                services.AddSingleton<IVirtualDesktopWrapperService, VirtualDesktopWrapperService>();
                services.AddSingleton<IVirtualDesktopService, VirtualDesktopService>();
                services.AddSingleton<IAlertService, AlertService>();
                services.AddSingleton<ICalloutNotificationService, CalloutNotificationService>();
                services.AddSingleton<ISubscriberService, SubscriberService>();
                services.AddSingleton<IUserResponseService, UserResponseService>();
                services.AddSingleton<ICalloutEmailNotificationService, CalloutEmailNotificationService>();
                services.AddSingleton<ILogCleanupService, LogCleanupService>();
                services.AddSingleton<IStartupService, StartupService>();
                services.AddSingleton<IAvailabilityStatusService, AvailabilityStatusService>();

                // Facades
                services.AddSingleton<IVirtualDesktopFacade, VirtualDesktopFacade>();
            }
            catch (Exception exception)
            {
                logFactory.Exception(exception, "Exception bei der Dependency Injection.");
            }
        }

        private static ILogFactory GetLogFactory(IConfigurationFileRepository configurationFileRepository, string appPart, string batchCommand)
        {
            var consoleLogService = new ConsoleLogService(appPart, batchCommand);
            var result = new LogFactory(consoleLogService, appPart, batchCommand);

            var dbLogService = new DbLogService(configurationFileRepository, appPart, batchCommand, consoleLogService);

            var emailService = new EmailService(result);
            var emailLogService = new EmailLogService(emailService, dbLogService, configurationFileRepository, appPart, batchCommand);

            result.LoggingServices.Add(dbLogService);
            result.LoggingServices.Add(emailLogService);
            return result;
        }

        private static List<Assembly> GetOwnAssemblies()
        {
            var result = new List<Assembly>();

            // Wichtig: Die hier ausgewählte "mainAssembly" muss über Referenzen auf alle benötigten Assemblies besitzen.
            // Assembly mainAsssembly = Assembly.GetEntryAssembly(); // Das ist die Assembly, die 
            Assembly mainAsssembly = typeof(DependencyInjector).Assembly;
            result.Add(mainAsssembly);
            foreach (AssemblyName referencedAssemblyName in mainAsssembly.GetReferencedAssemblies())
            {
                if (!referencedAssemblyName.Name.StartsWith("amtools", StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                result.Add(Assembly.Load(referencedAssemblyName));
            }
            return result;
        }
    }
}
