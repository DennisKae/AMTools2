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
using AMTools.Web.Core.Services.Interfaces;
using AMTools.Web.Core.Services.Synchronization.Database.Interfaces;
using AMTools.Web.Core.Services.Synchronization.JsonStore.Interfaces;
using AMTools.Web.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace AMTools.Web.BackgroundServices
{
    public class AvailabilityStatusBackgroundService : BaseFileChangeBackgroundService
    {
        private readonly IAvailabilityStatusDbSyncService _availabilityStatusDbSyncService;
        private readonly IConfigurationFileRepository _configurationFileRepository;
        private readonly IHubContext<AvailabilityHub> _hubContext;
        private readonly ISubscriberService _subscriberService;
        private readonly IAvailabilityStatusJsonStoreSyncService _availabilityStatusJsonStoreSyncService;

        public AvailabilityStatusBackgroundService(
            IAvailabilityStatusDbSyncService availabilityStatusDbSyncService,
            IConfigurationFileRepository configurationFileRepository,
            IHubContext<AvailabilityHub> hubContext,
            ISubscriberService subscriberService,
            IAvailabilityStatusJsonStoreSyncService availabilityStatusJsonStoreSyncService,
            ILogService logService) : base(logService)
        {
            _availabilityStatusDbSyncService = availabilityStatusDbSyncService;
            _configurationFileRepository = configurationFileRepository;
            _hubContext = hubContext;
            _subscriberService = subscriberService;
            _availabilityStatusJsonStoreSyncService = availabilityStatusJsonStoreSyncService;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            DateiKonfiguration dateiKonfiguration = _configurationFileRepository?.GetConfigFromJsonFile<DateiKonfiguration>();
            InitializeBackgroundService(dateiKonfiguration?.AvailabilityDatei);

            return Task.CompletedTask;
        }

        protected override void OnExceptionAfterFileChange()
        {
        }

        protected override void OnFileChange()
        {
            _availabilityStatusDbSyncService.Sync();
            _hubContext.Clients.All.SendAsync(nameof(AvailabilityHub.SendToAll), _subscriberService.GetAll());
            _availabilityStatusJsonStoreSyncService.Sync();
        }
    }
}