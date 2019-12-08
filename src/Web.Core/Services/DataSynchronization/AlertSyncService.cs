using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Shared.Core.Models;
using AMTools.Web.Data.Database;
using AMTools.Web.Data.Database.Models;
using AMTools.Web.Data.Database.Repositories;
using AMTools.Web.Data.Files.Repositories.Interfaces;
using AutoMapper;

namespace AMTools.Web.Core.Services.DataSynchronization
{
    public class AlertSyncService
    {
        private readonly ICalloutFileRepository _calloutFileRepository;
        private readonly IMapper _mapper;

        public AlertSyncService(
            ICalloutFileRepository calloutFileRepository,
            IMapper mapper)
        {
            _calloutFileRepository = calloutFileRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Erkennt neue Alerts
        /// </summary>
        public List<AlertIdentification> GetNewAlerts()
        {
            var result = new List<AlertIdentification>();
            List<AlertIdentification> fileAlertIdentifications = _calloutFileRepository.GetAllAlertIds();

            using (var unit = new UnitOfWork())
            {
                var dbRepo = unit.GetRepository<AlertDbRepository>();

                // Keine File-Alerts => Alle DB-Alerts deaktivieren
                if (fileAlertIdentifications == null || fileAlertIdentifications.Count == 0)
                {
                    dbRepo.DisableAll();
                    unit.SaveChanges();
                    return result;
                }

                // Keine aktive DB-Alerts vorhanden => Alle File-Alerts sind neu
                List<DbAlert> activeDbAlerts = dbRepo.GetEnabledAlerts();
                if (activeDbAlerts == null || activeDbAlerts.Count == 0)
                {
                    return fileAlertIdentifications;
                }

                // TODO: Einzelne File-Alerts auf Vorhandensein überprüfen und bei nicht-vorhandensein returnen
            }

            return result;
        }

        /// <summary>
        /// Ganz normaler Sync. 
        /// </summary>
        public void Sync()
        {
            //TODO: Wird das überhaupt gebraucht?
        }

        /// <summary>
        /// Deaktiviert DB-Alerts, die nicht mehr im Alert-File vorhanden sind
        /// </summary>
        public void DisableObsoleteAlerts()
        {

        }
    }
}
