using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Shared.Core.Models;
using AMTools.Shared.Core.Repositories.Interfaces;
using AMTools.Web.Core.Services.Interfaces;
using AMTools.Web.Core.ViewModels;
using AMTools.Web.Data.Database;
using AMTools.Web.Data.Database.Repositories;
using AutoMapper;

namespace AMTools.Web.Core.Services
{
    public class AlertService : IAlertService
    {
        private readonly IConfigurationFileRepository _configurationFileRepository;
        private readonly IMapper _mapper;

        public AlertService(
            IConfigurationFileRepository configurationFileRepository,
            IMapper mapper)
        {
            _configurationFileRepository = configurationFileRepository;
            _mapper = mapper;
        }

        public Alert GetLatestEnabledAlert()
        {
            using (var unit = new UnitOfWork(_configurationFileRepository))
            {
                var alertRepo = unit.GetRepository<AlertDbRepository>();
                return _mapper.Map<Alert>(alertRepo.GetLatestEnabledAlert());
            }
        }

        /// <summary>Liefert den Anlass der Alarmierung aus dem Alarmierungstext</summary>
        public string GetCalloutReasonFromAlertText(string alertText)
        {
            if (string.IsNullOrWhiteSpace(alertText) || !alertText.Contains('-'))
            {
                return null;
            }

            // 03.11. 20:29:33 - ID: 244, Schweregrad 6 - Musterhausen - Subgruppe(n): MUSTERHAUSEN_PAGER_VOLLALARM - S01*FUNKTIONSPROBE

            string[] splittedAlertText = alertText.Split('-');
            if (splittedAlertText.Length < 5)
            {
                return null;
            }

            var result = splittedAlertText[4].Trim();
            if (!string.IsNullOrWhiteSpace(splittedAlertText[4]))
            {
                return result;
            }

            return null;
        }

        public string GetTargetTextFromAlertText(string alertText)
        {
            if (string.IsNullOrWhiteSpace(alertText))
            {
                return null;
            }

            // 01.11. 21:22 - ID: 4, Schweregrad 1 - Jonas Färber -  - Funktionsprobe!

            string[] splittedAlertText = alertText.Split('-');
            if (splittedAlertText.Length < 3)
            {
                return null;
            }

            string alertTarget = splittedAlertText[2].Trim();
            if (string.IsNullOrWhiteSpace(alertTarget))
            {
                return null;
            }

            return alertTarget;
        }

        public AlertViewModel GetById(int alertId)
        {
            using (var unit = new UnitOfWork(_configurationFileRepository))
            {
                var alertRepo = unit.GetRepository<AlertDbRepository>();
                return _mapper.Map<AlertViewModel>(alertRepo.GetById(alertId));
            }
        }

        public AlertViewModel GetByAlertIdentification(AlertIdentification alertIdentification)
        {
            using (var unit = new UnitOfWork(_configurationFileRepository))
            {
                var alertRepo = unit.GetRepository<AlertDbRepository>();
                return _mapper.Map<AlertViewModel>(alertRepo.GetByAlertIdentification(alertIdentification));
            }
        }

        public void Disable(int alertId)
        {
            using (var unit = new UnitOfWork(_configurationFileRepository))
            {
                var alertRepo = unit.GetRepository<AlertDbRepository>();
                alertRepo.Disable(alertId);
            }
        }
    }
}
