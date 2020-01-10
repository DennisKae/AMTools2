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
    public class AlertSyncService : IAlertSyncService
    {
        private readonly ICalloutFileRepository _calloutFileRepository;
        private readonly IConfigurationFileRepository _configurationFileRepository;
        private readonly IMapper _mapper;

        public AlertSyncService(
            ICalloutFileRepository calloutFileRepository,
            IConfigurationFileRepository configurationFileRepository,
            IMapper mapper)
        {
            _calloutFileRepository = calloutFileRepository;
            _configurationFileRepository = configurationFileRepository;
            _mapper = mapper;
        }

        /// <summary>Erkennt neue Alerts</summary>
        public List<AlertIdentification> GetNewAlerts()
        {
            var result = new List<AlertIdentification>();
            List<AlertIdentification> fileAlertIdentifications = _calloutFileRepository.GetAllAlertIds();

            // Keine File-Alerts => Alle DB-Alerts deaktivieren
            if (fileAlertIdentifications == null || fileAlertIdentifications.Count == 0)
            {
                DisableAllAlerts();
                return result;
            }

            using (var unit = new UnitOfWork(_configurationFileRepository))
            {
                var dbRepo = unit.GetRepository<AlertDbRepository>();

                // Keine aktive DB-Alerts vorhanden => Alle File-Alerts sind neu
                List<DbAlert> activeDbAlerts = dbRepo.GetEnabledAlerts();
                if (activeDbAlerts == null || activeDbAlerts.Count == 0)
                {
                    return fileAlertIdentifications;
                }

                // Einzelne File-Alerts auf Vorhandensein überprüfen und bei nicht-vorhandensein zurückgeben
                foreach (AlertIdentification fileAlertIdentification in fileAlertIdentifications)
                {
                    DbAlert dbAlert = dbRepo.GetByAlertIdentification(fileAlertIdentification);
                    if (dbAlert == null)
                    {
                        result.Add(fileAlertIdentification);
                    }
                }
            }

            return result;
        }

        public void DisableAllAlerts()
        {
            using (var unit = new UnitOfWork(_configurationFileRepository))
            {
                var dbRepo = unit.GetRepository<AlertDbRepository>();
                dbRepo.DisableAll();
                unit.SaveChanges();
            }
        }

        public void ImportAlerts(List<AlertIdentification> alertIdentifications)
        {
            if (alertIdentifications == null || alertIdentifications.Count == 0)
            {
                return;
            }

            using (var unit = new UnitOfWork(_configurationFileRepository))
            {
                var dbRepo = unit.GetRepository<AlertDbRepository>();
                bool hasInserts = false;
                foreach (AlertIdentification alertIdentification in alertIdentifications)
                {
                    Alert fileAlert = _calloutFileRepository.GetAlert(alertIdentification);
                    if (fileAlert == null)
                    {
                        // Kein Datensatz zum Import gefunden
                        continue;
                    }

                    DbAlert dbAlert = dbRepo.GetByAlertIdentification(alertIdentification);
                    if (dbAlert != null)
                    {
                        // Alert existiert bereits in der DB
                        continue;
                    }

                    DbAlert newDbAlert = _mapper.Map<DbAlert>(fileAlert);
                    if (newDbAlert != null)
                    {
                        dbRepo.Insert(newDbAlert);
                        hasInserts = true;
                    }
                }

                if (hasInserts)
                {
                    unit.SaveChanges();
                }
            }
        }

        /// <summary>Deaktiviert DB-Alerts, die nicht mehr im Alert-File vorhanden sind</summary>
        public void DisableObsoleteAlerts()
        {
            using (var unit = new UnitOfWork(_configurationFileRepository))
            {
                var alertDbRepo = unit.GetRepository<AlertDbRepository>();
                List<DbAlert> activeDbAlerts = alertDbRepo.GetEnabledAlerts();
                if (activeDbAlerts == null || activeDbAlerts.Count == 0)
                {
                    return;
                }

                bool saveChanges = false;
                foreach (DbAlert activeDbAlert in activeDbAlerts)
                {
                    var alertIdentification = new AlertIdentification
                    {
                        Number = activeDbAlert.Number,
                        Timestamp = activeDbAlert.Timestamp
                    };

                    Alert fileAlert = _calloutFileRepository.GetAlert(alertIdentification);
                    if (fileAlert != null)
                    {
                        continue;
                    }

                    alertDbRepo.Disable(activeDbAlert.Id);
                    saveChanges = true;
                }

                if (saveChanges)
                {
                    unit.SaveChanges();
                }
            }
        }
    }
}
