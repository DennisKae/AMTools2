using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AMTools.Core.Services.Logging;
using AMTools.Shared.Core.Models.Konfigurationen;
using AMTools.Shared.Core.Repositories.Interfaces;
using AMTools.Web.Core.Services.Settings.Interfaces;
using AMTools.Web.Core.Services.Synchronization.Database.Interfaces;
using Microsoft.Extensions.Hosting;

namespace AMTools.Web.BackgroundServices
{
    public class SettingsBackgroundService : BaseFileChangeBackgroundService
    {
        private readonly ISettingsDbSyncService _settingsSyncService;
        private readonly IConfigurationFileRepository _configurationFileRepository;
        private readonly ISettingsService _settingsService;

        public SettingsBackgroundService(
            ISettingsDbSyncService settingsSyncService,
            IConfigurationFileRepository configurationFileRepository,
            ILogService logService,
            ISettingsService settingsService) : base(logService)
        {
            _settingsSyncService = settingsSyncService;
            _configurationFileRepository = configurationFileRepository;
            _settingsService = settingsService;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            DateiKonfiguration dateiKonfiguration = _configurationFileRepository?.GetConfigFromJsonFile<DateiKonfiguration>();
            InitializeBackgroundService(dateiKonfiguration?.SettingsDatei);

            return Task.CompletedTask;
        }

        protected override void OnExceptionAfterFileChange()
        {
        }

        protected override void OnFileChange()
        {
            _settingsSyncService.Sync();
            _settingsService.ClearMemoryCache();
        }
    }
}
