using System;
using System.Collections.Generic;
using System.Reflection;
using AMTools.Core.Services.Logging;
using AMTools.Shared.Core.Models;
using AMTools.Shared.Core.Repositories;
using AMTools.Shared.Core.Services.Logging;
using AMTools.Web.Core;
using AMTools.Web.Core.Services.DataSynchronization;
using AMTools.Web.Core.Services.Settings;
using AMTools.Web.Core.ViewModels;
using AMTools.Web.Data.Database;
using AMTools.Web.Data.Database.Models;
using AMTools.Web.Data.Files;
using AMTools.Web.Data.Files.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AMTools.Batch
{
    public static class Program
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "<Pending>")]
        public static void Main()
        {
            // TODO: DI nutzen https://andrewlock.net/using-dependency-injection-in-a-net-core-console-application/

            IMapper mapper = GetMapper();
            ILogService logService = GetLogService();
            /**
             * Import
             */
            ConfigurationFileRepository configFileRepo = new ConfigurationFileRepository();
            var calloutFileRepo = new CalloutFileRepository(logService, configFileRepo, mapper);
            //List<AlertIdentification> allAlertIds = calloutFileRepo.GetAllAlertIds();
            //Alert alert = calloutFileRepo.GetAlert(allAlertIds[2]);
            //List<UserResponse> responses = calloutFileRepo.GetUserResponses(allAlertIds[2]);

            var subscriberFileRepo = new SubscriberFileRepository(logService, configFileRepo);
            //List<Subscriber> allSubscribers = subscriberFileRepo.GetAll();


            var availabilityFileRepo = new AvailabilityFileRepository(logService, configFileRepo, mapper);
            //var availabilityByIssi = availabilityFileRepo.GetAvailabilityByIssi(allSubscribers[0].Issi);
            //var allAvailabilities = availabilityFileRepo.GetAllAvailabilities();

            var settingsFileRepo = new SettingsFileRepository(logService, configFileRepo, mapper);
            //var allSettings = settingsFileRepo.GetAllSettings();

            using (var context = new DatabaseContext())
            {
                context.Database.Migrate();
            }

            /**
             * Sync Services
             */

            //var settingsSyncService = new SettingsSyncService(logService, settingsFileRepo, mapper);
            //settingsSyncService.Sync();

            //var subscriberSyncService = new SubscriberSyncService(subscriberFileRepo, mapper);
            //subscriberSyncService.Sync();

            //var availabilityStatusSyncService = new AvailabilityStatusSyncService(availabilityFileRepo, mapper);
            //availabilityStatusSyncService.Sync();


            var settingsService = new SettingsService(mapper);
            var subGroupService = new SubGroupService(mapper, settingsService);
            List<SubGroupViewModel> allSubGroups = subGroupService.GetAll();
            var alertText = "03.11. 20:29:33 - ID: 244, Schweregrad 6 - Musterhausen - Subgruppe(n): MUSTERHAUSEN_PAGER_VOLLALARM - S01*FUNKTIONSPROBE";
            //List<SubGroupViewModel> detectedSubGroups1 = subGroupService.GetSubGroupsFromAlertText(alertText);

            var severityLevelService = new SeverityLevelService(mapper, settingsService);
            var severityLevel = severityLevelService.GetSeverityLevelFromAlertText(alertText);






            /*
             * Alert-Simulation
             */

            var alertSyncService = new AlertSyncService(calloutFileRepo, mapper);

            // Neue Alerts identifizieren
            List<AlertIdentification> newAlerts = alertSyncService.GetNewAlerts();

            if (newAlerts?.Count > 0)
            {
                // Bildschirm umschalten


                // Neue Alerts importieren
                alertSyncService.ImportAlerts(newAlerts);

                // Benachrichtigungen versenden
            }

            // UserResponse Updates verarbeiten
            var userResponseSyncService = new UserResponseSyncService(mapper, calloutFileRepo);
            List<DbUserResponse> newUserResponses = userResponseSyncService.SyncAndGetNewUserResponses();
            // Benachrichtigungen über neue UserResponses versenden

            // Obsolete Alerts deaktivieren
            alertSyncService.DisableObsoleteAlerts();
        }


        private static IMapper GetMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<FileImportMapProfile>();
                cfg.AddProfile<DatabaseMapProfile>();
                cfg.AddProfile<ViewModelMapProfile>();
            });

            return new Mapper(config);
        }

        private static ILogService GetLogService()
        {
            return new ConsoleLogService(Assembly.GetExecutingAssembly().GetName().Name, null);
        }
    }
}
