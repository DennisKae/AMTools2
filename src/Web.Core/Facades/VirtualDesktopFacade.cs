using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Core.Services.Logging;
using AMTools.Shared.Core;
using AMTools.Shared.Core.Models;
using AMTools.Shared.Core.Models.Konfigurationen;
using AMTools.Shared.Core.Repositories.Interfaces;
using AMTools.Shared.Core.Services.VirtualDesktops.Interfaces;
using AMTools.Web.Core.Facades.Interfaces;
using AMTools.Web.Core.Services.Interfaces;

namespace AMTools.Web.Core.Facades
{
    public class VirtualDesktopFacade : IVirtualDesktopFacade
    {
        private readonly ILogService _logService;
        private readonly IVirtualDesktopService _virtualDesktopService;
        private readonly IAlertService _alertService;
        private readonly IConfigurationFileRepository _configurationFileRepository;

        public VirtualDesktopFacade(
            ILogService logService,
            IVirtualDesktopService virtualDesktopService,
            IAlertService alertService,
            IConfigurationFileRepository configurationFileRepository)
        {
            _logService = logService;
            _virtualDesktopService = virtualDesktopService;
            _alertService = alertService;
            _configurationFileRepository = configurationFileRepository;
        }

        public void SwitchRight()
        {
            if (!SwitchingIsAllowed())
            {
                return;
            }

            _virtualDesktopService.SwitchRight();
        }

        public void SwitchLeft()
        {
            if (!SwitchingIsAllowed())
            {
                return;
            }

            _virtualDesktopService.SwitchLeft();
        }

        private bool SwitchingIsAllowed()
        {
            Alert latestAlert = _alertService.GetLatestEnabledAlert();
            if (latestAlert == null)
            {
                return true;
            }

            AlarmKonfiguration alarmKonfiguration = _configurationFileRepository.GetConfigFromJsonFile<AlarmKonfiguration>();
            Guard.IsNotNull(alarmKonfiguration, nameof(AlarmKonfiguration));

            double minutenSeitLetzterAlarmierung = Math.Round((DateTime.Now - latestAlert.AlertTimestamp).TotalMinutes, 2);
            if (minutenSeitLetzterAlarmierung < alarmKonfiguration.SperrfristInMinuten)
            {
                double endeDerSperrfristInMinuten = alarmKonfiguration.SperrfristInMinuten - minutenSeitLetzterAlarmierung;
                _logService.Info($"Kein Desktopwechsel erlaubt: Seit der letzten Alarmierung sind erst {minutenSeitLetzterAlarmierung} Minuten vergangen. Der nächste Wechsel ist erst in {endeDerSperrfristInMinuten} Minuten erlaubt.");
                return false;
            }

            return true;
        }
    }
}
