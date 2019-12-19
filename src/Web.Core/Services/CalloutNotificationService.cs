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
using AMTools.Web.Core.Services.Interfaces;
using AMTools.Web.Core.ViewModels;
using AMTools.Web.Data.Database.Models;

namespace AMTools.Web.Core.Services
{
    public class CalloutNotificationService : ICalloutNotificationService
    {
        private readonly IConfigurationFileRepository _configurationFileRepository;
        private readonly ILogService _logService;
        private readonly IAlertService _alertService;

        public CalloutNotificationService(
            IConfigurationFileRepository configurationFileRepository,
            ILogService logService,
            IAlertService alertService
            )
        {
            _configurationFileRepository = configurationFileRepository;
            _logService = logService;
            _alertService = alertService;
        }

        /// <summary>Versendet Benachrichtigungen über neue Alarmierungen.</summary>
        public void SendAlertNotifications(List<AlertIdentification> alertIdentifications)
        {
            if (alertIdentifications == null || alertIdentifications.Count == 0)
            {
                return;
            }

            try
            {
                AlarmKonfiguration alarmKonfiguration = _configurationFileRepository.GetConfigFromJsonFile<AlarmKonfiguration>();
                Guard.IsNotNull(alarmKonfiguration, nameof(AlarmKonfiguration));
                alertIdentifications.ForEach(x => SendAlertNotification(x, alarmKonfiguration));
            }
            catch (Exception exception)
            {
                _logService.Exception(exception, "@" + nameof(SendAlertNotifications));
            }
        }

        /// <summary>Versendet Benachrichtigungen über neue UserResponses.</summary>
        public void SendNewUserResponseNotifications(List<DbUserResponse> userResponses)
        {
            if (userResponses == null || userResponses.Count == 0)
            {
                return;
            }

            try
            {
                AlarmKonfiguration alarmKonfiguration = _configurationFileRepository.GetConfigFromJsonFile<AlarmKonfiguration>();
                Guard.IsNotNull(alarmKonfiguration, nameof(AlarmKonfiguration));
                List<int> alertIds = userResponses.Select(x => x.AlertId).Distinct().OrderByDescending(x => x).ToList();

                alertIds.ForEach(x => SendNewUserResponseNotification(x, alarmKonfiguration));
            }
            catch (Exception exception)
            {
                _logService.Exception(exception, "@" + nameof(SendNewUserResponseNotifications));
            }
        }

        private void SendAlertNotification(AlertIdentification alertIdentification, AlarmKonfiguration alarmKonfiguration)
        {
            // Quasi das Gleiche wie bei neuen UserResponses nur mit anderem Betreff

            AlertViewModel alert = _alertService.GetByAlertIdentification(alertIdentification);
            if (alert == null)
            {
                _logService.Error(nameof(CalloutNotificationService) + "." + nameof(SendAlertNotification) + ": Kein alert zu dieser Identifikation gefunden: " + alertIdentification?.ToString());
                return;
            }
            var test = alertIdentification?.ToString();
            var headline = "Neue Alarmierung - " + alert.Text;
        }

        private void SendNewUserResponseNotification(int alertId, AlarmKonfiguration alarmKonfiguration)
        {
            // var dbAlert =
            // Als Header/Einleitung die allgemeinen Infos zur Alarmierung
            // Darunter eine Übersicht mit den zurückgemeldeten Qualifizierungen und deren Anzahlen
            // Darunter eine Tabelle mit den Rückmeldungen (durchnummeriert, nach Timestamp absteigend sortiert)

            AlertViewModel alert = _alertService.GetById(alertId);
        }


        private void SendCalloutSummaryEmail(AlertViewModel alert, string headline)
        {

        }
    }
}
