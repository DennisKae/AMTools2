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
using AutoMapper;

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

        private System.Timers.Timer _switchTimer;

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

        protected override void OnExceptionAfterFileChange() => SwitchWithTimeout();

        protected override void OnFileChange()
        {
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
                AlarmKonfiguration alarmKonfiguration = _configurationFileRepository.GetConfigFromJsonFile<AlarmKonfiguration>();
                if (alarmKonfiguration == null)
                {
                    _logService.Error($"Keine {nameof(AlarmKonfiguration)} gefunden! Es wird stattdessen eine Fallbackkonfiguration verwendet.");
                    alarmKonfiguration = FallbackKonfigurationen.AlarmKonfiguration;
                }

                // Bildschirm umschalten
                if (IsInitialized)
                {
                    SwitchWithTimeout();
                }

                // Neue Alerts importieren
                _alertSyncService.ImportAlerts(newAlerts);

                // Benachrichtigungen versenden
                _calloutNotificationService.SendNewAlertNotifications(newAlerts);
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
            DateiKonfiguration dateiKonfiguration = _configurationFileRepository.GetConfigFromJsonFile<DateiKonfiguration>();
            return string.IsNullOrWhiteSpace(dateiKonfiguration?.CalloutDatei) || !File.Exists(dateiKonfiguration.CalloutDatei) || string.IsNullOrWhiteSpace(File.ReadAllText(dateiKonfiguration.CalloutDatei));
        }

        private AlarmKonfiguration GetSafeAlarmkonfiguration()
        {
            AlarmKonfiguration alarmKonfiguration = _configurationFileRepository.GetConfigFromJsonFile<AlarmKonfiguration>();
            if (alarmKonfiguration == null)
            {
                _logService.Error($"Keine {nameof(AlarmKonfiguration)} gefunden! Es wird stattdessen eine Fallbackkonfiguration verwendet.");
                alarmKonfiguration = FallbackKonfigurationen.AlarmKonfiguration;
            }

            if (!alarmKonfiguration.AlarmierungsDesktop.HasValue)
            {
                _logService.Error($"{nameof(AlarmKonfiguration)}: Kein {nameof(alarmKonfiguration.AlarmierungsDesktop)} gefunden. Der Wert \"{FallbackKonfigurationen.AlarmKonfiguration.AlarmierungsDesktop}\" wird verwendet.");
                alarmKonfiguration.AlarmierungsDesktop = FallbackKonfigurationen.AlarmKonfiguration.AlarmierungsDesktop;
            }

            if (!alarmKonfiguration.StandbyDesktop.HasValue)
            {
                _logService.Error($"{nameof(AlarmKonfiguration)}: Kein {nameof(alarmKonfiguration.StandbyDesktop)} gefunden. Der Wert \"{FallbackKonfigurationen.AlarmKonfiguration.StandbyDesktop}\" wird verwendet.");
                alarmKonfiguration.StandbyDesktop = FallbackKonfigurationen.AlarmKonfiguration.StandbyDesktop;
            }

            if (!alarmKonfiguration.SperrfristInMinuten.HasValue)
            {
                _logService.Error($"{nameof(AlarmKonfiguration)}: Keine {nameof(alarmKonfiguration.SperrfristInMinuten)} gefunden. Der Wert \"{FallbackKonfigurationen.AlarmKonfiguration.SperrfristInMinuten}\" wird verwendet.");
                alarmKonfiguration.SperrfristInMinuten = FallbackKonfigurationen.AlarmKonfiguration.SperrfristInMinuten;
            }

            return alarmKonfiguration;
        }



        private void SwitchWithTimeout()
        {
            // Timeout starten und anschließend wieder auf den Standby-Monitor zurückswitchen
            var alarmKonfiguration = GetSafeAlarmkonfiguration();

            _virtualDesktopService.SwitchWithMultipleAttempts(alarmKonfiguration.AlarmierungsDesktop.Value, nameof(alarmKonfiguration.AlarmierungsDesktop));

            bool isRestart = false;
            if (_switchTimer == null)
            {
                _switchTimer = new System.Timers.Timer();
                _switchTimer.Elapsed += SwitchTimerElapsed;
            }
            else
            {
                _switchTimer.Stop();
                isRestart = true;
            }

            _switchTimer.Interval = alarmKonfiguration.SperrfristInMinuten.Value * 60 * 1000;
            _switchTimer.AutoReset = false;
            _switchTimer.Enabled = true;
            _switchTimer.Start();

            var logMessageSuffix = $" wurde zum {nameof(alarmKonfiguration.AlarmierungsDesktop)} ({alarmKonfiguration.AlarmierungsDesktop}) gewechselt und in {alarmKonfiguration.SperrfristInMinuten} Minuten wird zurück auf den {nameof(alarmKonfiguration.StandbyDesktop)} gewechselt.";
            if (isRestart)
            {
                _logService.Info("Es" + logMessageSuffix);
            }
            else
            {
                _logService.Info("Der SwitchTimer wurde angehalten. Anschließend" + logMessageSuffix);
            }
        }

        private void SwitchTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            AlarmKonfiguration alarmKonfiguration = GetSafeAlarmkonfiguration();
            _logService.Info($"Timer abgelaufen. Es wird zum Desktop {alarmKonfiguration.StandbyDesktop} gewechselt.");
            _virtualDesktopService.SwitchWithMultipleAttempts(alarmKonfiguration.StandbyDesktop.Value, nameof(alarmKonfiguration.StandbyDesktop));
        }
    }
}
