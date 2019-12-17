using System;
using System.Collections.Generic;
using System.Reflection;
using AMTools.Core.Services.Logging;
using AMTools.Shared.Core.Models;
using AMTools.Shared.Core.Repositories;
using AMTools.Shared.Core.Services.Logging;
using AMTools.Web.Core.Services.DataSynchronization;
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

            var settingsSyncService = new SettingsSyncService(logService, settingsFileRepo, mapper);
            settingsSyncService.Sync();

            var subscriberSyncService = new SubscriberSyncService(subscriberFileRepo, mapper);
            subscriberSyncService.Sync();

            var availabilityStatusSyncService = new AvailabilityStatusSyncService(availabilityFileRepo, mapper);
            availabilityStatusSyncService.Sync();


            /*
             * Alert-Simulation
             */

            var alertSyncService = new AlertSyncService(calloutFileRepo, mapper);

            // Neue Alerts identifizieren
            List<AlertIdentification> newAlerts = alertSyncService.GetNewAlerts();

            if (newAlerts?.Count > 0)
            {
                // TODO: Bildschirm umschalten


                // Neue Alerts importieren
                alertSyncService.ImportAlerts(newAlerts);

                // TODO: Benachrichtigungen versenden
            }

            // UserResponse Updates verarbeiten
            var userResponseSyncService = new UserResponseSyncService(mapper, calloutFileRepo);
            List<DbUserResponse> newUserResponses = userResponseSyncService.SyncAndGetNewUserResponses();
            // TODO: Benachrichtigungen der neuen UserResponses versenden

            // Obsolete Alerts deaktivieren
            alertSyncService.DisableObsoleteAlerts();
        }


        private static IMapper GetMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<FileImportMapProfile>();
                cfg.AddProfile<DatabaseMapProfile>();
            });

            return new Mapper(config);
        }

        private static ILogService GetLogService()
        {
            return new ConsoleLogService(Assembly.GetExecutingAssembly().FullName, null);
        }
    }
}
