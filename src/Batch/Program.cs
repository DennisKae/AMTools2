using System;
using System.Collections.Generic;
using AMTools.Shared.Core.Models;
using AMTools.Web.Core.Services.DataSynchronization;
using AMTools.Web.Data.Database;
using AMTools.Web.Data.Files;
using AMTools.Web.Data.Files.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AMTools.Batch
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            //const string calloutFilePath = @"C:\Users\Dennis\source\repos\AMTools2\ressources\callout.hst";
            IMapper mapper = GetMapper();

            //var calloutImportRepo = new CalloutFileRepository(calloutFilePath, mapper);
            //List<AlertIdentification> allAlertIds = calloutImportRepo.GetAllAlertIds();
            //Alert alert = calloutImportRepo.GetAlert(allAlertIds[2]);
            //List<UserResponse> responses = calloutImportRepo.GetUserResponses(allAlertIds[2]);

            const string subscriberFilePath = @"C:\Users\Dennis\source\repos\AMTools2\ressources\subscribers.xml";
            var subscriberFileRepo = new SubscriberFileRepository(subscriberFilePath);
            //List<Subscriber> allSubscribers = subscriberFileRepo.GetAllSubscribers();


            //const string availabilityFilePath = @"C:\Users\Dennis\source\repos\AMTools2\ressources\availability.hst";
            //var availabilityFileRepo = new AvailabilityFileRepository(availabilityFilePath, mapper);
            //var availabilityByIssi = availabilityFileRepo.GetAvailabilityByIssi(allSubscribers[0].Issi);
            //var allAvailabilities = availabilityFileRepo.GetAllAvailabilities();

            const string settingsFilePath = @"C:\Users\Dennis\source\repos\AMTools2\ressources\settings.xml";
            var settingsFileRepo = new SettingsFileRepository(settingsFilePath, mapper);
            //var allSettings = settingsFileRepo.GetAllSettings();

            var context = new DatabaseContext();
            context.Database.Migrate();

            var settingsSyncService = new SettingsSyncService(settingsFileRepo, mapper);
            settingsSyncService.Sync();

            var subscriberSyncService = new SubscriberSyncService(subscriberFileRepo, mapper);
            subscriberSyncService.Sync();
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
