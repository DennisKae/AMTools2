using System;
using System.Collections.Generic;
using AMTools.Shared.Core.Models;
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

            /**
             * Import
             */

            const string calloutFilePath = @"C:\Users\Dennis\source\repos\AMTools2\ressources\callout.hst";
            var calloutFileRepo = new CalloutFileRepository(calloutFilePath, mapper);
            //List<AlertIdentification> allAlertIds = calloutImportRepo.GetAllAlertIds();
            //Alert alert = calloutImportRepo.GetAlert(allAlertIds[2]);
            //List<UserResponse> responses = calloutImportRepo.GetUserResponses(allAlertIds[2]);

            //const string subscriberFilePath = @"C:\Users\Dennis\source\repos\AMTools2\ressources\subscribers.xml";
            //var subscriberFileRepo = new SubscriberFileRepository(subscriberFilePath);
            //List<Subscriber> allSubscribers = subscriberFileRepo.GetAllSubscribers();


            //const string availabilityFilePath = @"C:\Users\Dennis\source\repos\AMTools2\ressources\availability.hst";
            //var availabilityFileRepo = new AvailabilityFileRepository(availabilityFilePath, mapper);
            //var availabilityByIssi = availabilityFileRepo.GetAvailabilityByIssi(allSubscribers[0].Issi);
            //var allAvailabilities = availabilityFileRepo.GetAllAvailabilities();

            const string settingsFilePath = @"C:\Users\Dennis\source\repos\AMTools2\ressources\settings.xml";
            var settingsFileRepo = new SettingsFileRepository(settingsFilePath, mapper);
            //var allSettings = settingsFileRepo.GetAllSettings();

            using (var context = new DatabaseContext())
            {
                context.Database.Migrate();
            }

            /**
             * Sync Services
             */

            var settingsSyncService = new SettingsSyncService(settingsFileRepo, mapper);
            settingsSyncService.Sync();

            //var subscriberSyncService = new SubscriberSyncService(subscriberFileRepo, mapper);
            //subscriberSyncService.Sync();

            //var availabilityStatusSyncService = new AvailabilityStatusSyncService(availabilityFileRepo, mapper);
            //availabilityStatusSyncService.Sync();


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
    }
}
