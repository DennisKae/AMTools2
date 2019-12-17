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
using AMTools.Web.Core.Services.DataSynchronization.Interfaces;
using AMTools.Web.Data.Database.Models;

namespace AMTools.Web.BackgroundServices
{
    public class CalloutBackgroundService : BaseFileChangeBackgroundService
    {
        private readonly IAlertSyncService _alertSyncService;
        private readonly IUserResponseSyncService _userResponseSyncService;
        private readonly IConfigurationFileRepository _configurationFileRepository;

        public CalloutBackgroundService(
            IAlertSyncService alertSyncService,
            IUserResponseSyncService userResponseSyncService,
            IConfigurationFileRepository configurationFileRepository,
            ILogService logService) : base(logService)
        {
            _alertSyncService = alertSyncService;
            _userResponseSyncService = userResponseSyncService;
            _configurationFileRepository = configurationFileRepository;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            DateiKonfiguration dateiKonfiguration = _configurationFileRepository?.GetConfigFromJsonFile<DateiKonfiguration>();
            InitializeBackgroundService(dateiKonfiguration?.CalloutDatei);

            return Task.CompletedTask;
        }

        protected override void OnFileChange()
        {
            // Neue Alerts identifizieren
            List<AlertIdentification> newAlerts = _alertSyncService.GetNewAlerts();

            if (newAlerts?.Count > 0)
            {
                // TODO: Bildschirm umschalten


                // Neue Alerts importieren
                _alertSyncService.ImportAlerts(newAlerts);

                // TODO: Benachrichtigungen versenden
            }

            // UserResponse Updates verarbeiten
            List<DbUserResponse> newUserResponses = _userResponseSyncService.SyncAndGetNewUserResponses();
            // TODO: Benachrichtigungen über neue UserResponses versenden

            // Obsolete Alerts deaktivieren
            _alertSyncService.DisableObsoleteAlerts();
        }
    }
}
