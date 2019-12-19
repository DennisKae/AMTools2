using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AMTools.Core.Services.Logging;
using AMTools.Shared.Core.Models;
using AMTools.Shared.Core.Models.Konfigurationen;
using AMTools.Shared.Core.Repositories.Interfaces;
using AMTools.Shared.Core.Services.VirtualDesktops.Interfaces;
using AMTools.Web.Core.Services.DataSynchronization.Interfaces;
using AMTools.Web.Core.Services.Interfaces;
using AMTools.Web.Data.Database.Models;

namespace AMTools.Web.BackgroundServices
{
    public class CalloutBackgroundService : BaseFileChangeBackgroundService
    {
        private readonly IAlertSyncService _alertSyncService;
        private readonly IUserResponseSyncService _userResponseSyncService;
        private readonly IConfigurationFileRepository _configurationFileRepository;
        private readonly IVirtualDesktopService _virtualDesktopService;
        private readonly ICalloutNotificationService _calloutNotificationService;
        private readonly ILogService _logService;

        public CalloutBackgroundService(
            IAlertSyncService alertSyncService,
            IUserResponseSyncService userResponseSyncService,
            IConfigurationFileRepository configurationFileRepository,
            IVirtualDesktopService virtualDesktopService,
            ICalloutNotificationService calloutNotificationService,
            ILogService logService) : base(logService)
        {
            _alertSyncService = alertSyncService;
            _userResponseSyncService = userResponseSyncService;
            _configurationFileRepository = configurationFileRepository;
            _virtualDesktopService = virtualDesktopService;
            _calloutNotificationService = calloutNotificationService;
            _logService = logService;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            DateiKonfiguration dateiKonfiguration = _configurationFileRepository?.GetConfigFromJsonFile<DateiKonfiguration>();
            InitializeBackgroundService(dateiKonfiguration?.CalloutDatei);

            return Task.CompletedTask;
        }

        protected override void OnExceptionAfterFileChange()
        {
            AlarmKonfiguration alarmKonfiguration = _configurationFileRepository.GetConfigFromJsonFile<AlarmKonfiguration>();
            if (alarmKonfiguration == null)
            {
                _logService.Error($"Keine {nameof(AlarmKonfiguration)} gefunden! Es wird stattdessen eine Fallbackkonfiguration verwendet.");
                alarmKonfiguration = FallbackKonfigurationen.AlarmKonfiguration;
            }
            SwitchWithTimeout(alarmKonfiguration);
        }

        protected override void OnFileChange()
        {
            AlarmKonfiguration alarmKonfiguration = _configurationFileRepository.GetConfigFromJsonFile<AlarmKonfiguration>();

            if (CalloutFileIsEmpty())
            {
                // Wenn die Datei leer ist: Loggen und abbrechen
                _logService.Info(nameof(CalloutBackgroundService) + ": Die überwachte Datei ist leer. Es findet keine weitere Verarbeitung statt.");
                return;
            }

            // Neue Alerts identifizieren
            List<AlertIdentification> newAlerts = _alertSyncService.GetNewAlerts();

            if (newAlerts?.Count > 0)
            {
                if (alarmKonfiguration == null)
                {
                    _logService.Error($"Keine {nameof(AlarmKonfiguration)} gefunden! Es wird stattdessen eine Fallbackkonfiguration verwendet.");
                    alarmKonfiguration = FallbackKonfigurationen.AlarmKonfiguration;
                }

                // Bildschirm umschalten
                SwitchWithTimeout(alarmKonfiguration);

                // Neue Alerts importieren
                _alertSyncService.ImportAlerts(newAlerts);

                // Benachrichtigungen versenden
                _calloutNotificationService.SendAlertNotifications(newAlerts);
            }

            // UserResponse Updates verarbeiten TODO: Sync XML Prop in der Alert Tabelle
            List<DbUserResponse> newUserResponses = _userResponseSyncService.SyncAndGetNewUserResponses();

            // Benachrichtigungen über neue UserResponses versenden
            if (newUserResponses?.Count > 0)
            {
                _calloutNotificationService.SendNewUserResponseNotifications(newUserResponses);
            }

            // Obsolete Alerts deaktivieren
            _alertSyncService.DisableObsoleteAlerts();
        }

        private bool CalloutFileIsEmpty()
        {
            return true;
        }

        private void SwitchWithTimeout(AlarmKonfiguration alarmKonfiguration)
        {
            _virtualDesktopService.Switch(alarmKonfiguration?.AlarmierungsDesktop ?? 1 - 1);

            //TODO: Timeout starten und anschließend wieder auf den Standby-Monitor zurückswitchen
        }
    }
}
