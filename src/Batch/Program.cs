using System;
using System.Collections.Generic;
using AMTools.Shared.Core.Models;
using AMTools.Web.Data.Files;
using AMTools.Web.Data.Files.Repositories;
using AutoMapper;

namespace AMTools.Batch
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            const string calloutFilePath = @"C:\Users\Dennis\source\repos\AMTools2\ressources\callout.hst";
            IMapper mapper = GetMapper();

            var calloutImportRepo = new CalloutFileImportRepository(calloutFilePath, mapper);
            List<AlertIdentification> allAlertIds = calloutImportRepo.GetAllAlertIds();
            Alert alert = calloutImportRepo.GetAlert(allAlertIds[2]);
            List<UserResponse> responses = calloutImportRepo.GetUserResponses(allAlertIds[2]);
        }


        private static IMapper GetMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<FileImportMapProfile>();
            });

            return new Mapper(config);
        }
    }
}
