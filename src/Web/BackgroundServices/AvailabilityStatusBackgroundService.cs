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
using AMTools.Web.Core.Services.DataSynchronization.Interfaces;

namespace AMTools.Web.BackgroundServices
{
    public class AvailabilityStatusBackgroundService : BaseFileChangeBackgroundService
    {
        private readonly IAvailabilityStatusSyncService _availabilityStatusSyncService;
        private readonly IConfigurationFileRepository _configurationFileRepository;

        public AvailabilityStatusBackgroundService(
            IAvailabilityStatusSyncService availabilityStatusSyncService,
            IConfigurationFileRepository configurationFileRepository,
            ILogService logService) : base(logService)
        {
            _availabilityStatusSyncService = availabilityStatusSyncService;
            _configurationFileRepository = configurationFileRepository;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            DateiKonfiguration dateiKonfiguration = _configurationFileRepository?.GetConfigFromJsonFile<DateiKonfiguration>();
            InitializeFileSystemWatcher(dateiKonfiguration?.AvailabilityDatei);

            return Task.CompletedTask;
        }

        protected override void OnFileChange(object sender, FileSystemEventArgs eventArgs)
        {
            _availabilityStatusSyncService.Sync();
        }
    }
}