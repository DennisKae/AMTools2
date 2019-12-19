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
        private readonly ICalloutEmailNotificationService _calloutEmailNotificationService;

        public CalloutNotificationService(
            IConfigurationFileRepository configurationFileRepository,
            ILogService logService,
            IAlertService alertService,
            ICalloutEmailNotificationService calloutEmailNotificationService
            )
        {
            _configurationFileRepository = configurationFileRepository;
            _logService = logService;
            _alertService = alertService;
            _calloutEmailNotificationService = calloutEmailNotificationService;
        }

        /// <summary>Versendet Benachrichtigungen über neue Alarmierungen.</summary>
        public void SendNewAlertNotifications(List<AlertIdentification> alertIdentifications)
        {
            if (alertIdentifications == null || alertIdentifications.Count == 0)
            {
                return;
            }
            AlarmKonfiguration alarmKonfiguration = _configurationFileRepository.GetConfigFromJsonFile<AlarmKonfiguration>();

            foreach (AlertIdentification alertIdentification in alertIdentifications)
            {
                try
                {
                    Guard.IsNotNull(alarmKonfiguration, nameof(AlarmKonfiguration));

                    AlertViewModel alert = _alertService.GetByAlertIdentification(alertIdentification);
                    if (alert == null)
                    {
                        _logService.Error(nameof(SendNewAlertNotifications) + ": Kein alert zu dieser Identifikation gefunden: " + alertIdentification?.ToString());
                        return;
                    }
                    var test = alertIdentification?.ToString();

                    _calloutEmailNotificationService.SendEmail(alert, CalloutNotificationType.NewAlert);
                }
                catch (Exception exception)
                {
                    _logService.Exception(exception, "@" + nameof(SendNewAlertNotifications) + ": AlertIdentificaton " + alertIdentification?.ToString());
                }
            }
        }

        /// <summary>Versendet Benachrichtigungen über neue UserResponses.</summary>
        public void SendNewUserResponseNotifications(List<DbUserResponse> userResponses)
        {
            if (userResponses == null || userResponses.Count == 0)
            {
                return;
            }

            AlarmKonfiguration alarmKonfiguration = _configurationFileRepository.GetConfigFromJsonFile<AlarmKonfiguration>();
            Guard.IsNotNull(alarmKonfiguration, nameof(AlarmKonfiguration));
            List<int> alertIds = userResponses.Select(x => x.AlertId).Distinct().OrderByDescending(x => x).ToList();
            foreach (int alertId in alertIds)
            {
                try
                {
                    AlertViewModel alert = _alertService.GetById(alertId);
                    if (alert == null)
                    {
                        _logService.Error(nameof(SendNewUserResponseNotifications) + ": Kein alert zu dieser AlertId gefunden: " + alertId);
                        return;
                    }

                    _calloutEmailNotificationService.SendEmail(alert, CalloutNotificationType.NewUserResponse);
                }
                catch (Exception exception)
                {
                    _logService.Exception(exception, "@" + nameof(SendNewUserResponseNotifications) + ": AlertId " + alertId);
                }
            }
        }
    }
}
