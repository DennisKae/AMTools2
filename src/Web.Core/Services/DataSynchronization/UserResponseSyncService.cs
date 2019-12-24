using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Shared.Core.Models;
using AMTools.Shared.Core.Repositories.Interfaces;
using AMTools.Web.Core.Services.DataSynchronization.Interfaces;
using AMTools.Web.Data.Database;
using AMTools.Web.Data.Database.Models;
using AMTools.Web.Data.Database.Repositories;
using AMTools.Web.Data.Files.Repositories.Interfaces;
using AutoMapper;

namespace AMTools.Web.Core.Services.DataSynchronization
{
    public class UserResponseSyncService : IUserResponseSyncService
    {
        private readonly IConfigurationFileRepository _configurationFileRepository;
        private readonly IMapper _mapper;
        private readonly ICalloutFileRepository _calloutFileRepository;

        public UserResponseSyncService(
            IConfigurationFileRepository configurationFileRepository,
            IMapper mapper,
            ICalloutFileRepository calloutFileRepository)
        {
            _configurationFileRepository = configurationFileRepository;
            _mapper = mapper;
            _calloutFileRepository = calloutFileRepository;
        }

        /// <summary>
        /// Importiert neue UserResponses. Änderungen werden nicht erkannt.
        /// </summary>
        public List<DbUserResponse> SyncAndGetNewUserResponses()
        {
            var result = new List<DbUserResponse>();

            List<AlertIdentification> fileAlertIdentifications = _calloutFileRepository.GetAllAlertIds();
            if (fileAlertIdentifications == null || fileAlertIdentifications.Count == 0)
            {
                return result;
            }

            using (var unit = new UnitOfWork(_configurationFileRepository))
            {
                var alertRepo = unit.GetRepository<AlertDbRepository>();
                var userResponseRepo = unit.GetRepository<UserResponseDbRepository>();
                bool saveChanges = false;

                foreach (AlertIdentification alertIdentification in fileAlertIdentifications)
                {
                    List<UserResponse> fileUserResponses = _calloutFileRepository.GetUserResponses(alertIdentification);
                    if (fileUserResponses == null || fileUserResponses.Count == 0)
                    {
                        // Keine UserResponses in der Datei vorhanden
                        continue;
                    }

                    DbAlert dbAlert = alertRepo.GetByAlertIdentification(alertIdentification);
                    if (dbAlert == null)
                    {
                        // In der DB nicht vorhandener Alert
                        continue;
                    }

                    List<DbUserResponse> existingDbResponses = userResponseRepo.GetByAlertId(dbAlert.Id);
                    if (existingDbResponses == null || existingDbResponses.Count == 0)
                    {
                        // Noch keine UserResponses in der DB
                        List<DbUserResponse> mappedResponses = _mapper.Map<List<DbUserResponse>>(fileUserResponses);
                        mappedResponses.ForEach(x => x.AlertId = dbAlert.Id);
                        userResponseRepo.Insert(mappedResponses);
                        result.AddRange(mappedResponses);
                        saveChanges = true;
                        continue;
                    }

                    foreach (UserResponse fileUserRespone in fileUserResponses)
                    {
                        if (existingDbResponses.Any(x => x.Issi == fileUserRespone.Issi))
                        {
                            // Eine Antwort für diesen User existiert bereits
                            continue;
                        }

                        DbUserResponse mappedResponse = _mapper.Map<DbUserResponse>(fileUserRespone);
                        mappedResponse.AlertId = dbAlert.Id;
                        userResponseRepo.Insert(mappedResponse);
                        result.Add(mappedResponse);
                        saveChanges = true;
                    }
                }

                if (saveChanges)
                {
                    unit.SaveChanges();
                }
            }

            return result;
        }
    }
}
