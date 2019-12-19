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
    public class SubscriberBackgroundService : BaseFileChangeBackgroundService
    {
        private readonly ISubscriberSyncService _subscriberSyncService;
        private readonly IConfigurationFileRepository _configurationFileRepository;

        public SubscriberBackgroundService(
            ISubscriberSyncService subscriberSyncService,
            IConfigurationFileRepository configurationFileRepository,
            ILogService logService) : base(logService)
        {
            _subscriberSyncService = subscriberSyncService;
            _configurationFileRepository = configurationFileRepository;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            DateiKonfiguration dateiKonfiguration = _configurationFileRepository?.GetConfigFromJsonFile<DateiKonfiguration>();
            InitializeBackgroundService(dateiKonfiguration?.SubscriberDatei);

            return Task.CompletedTask;
        }

        protected override void OnExceptionAfterFileChange()
        {
        }

        protected override void OnFileChange()
        {
            _subscriberSyncService.Sync();
        }
    }
}
