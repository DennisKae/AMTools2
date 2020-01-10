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
using AMTools.Web.Core.Services.Interfaces;
using AMTools.Web.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace AMTools.Web.BackgroundServices
{
    public class SubscriberBackgroundService : BaseFileChangeBackgroundService
    {
        private readonly ISubscriberSyncService _subscriberSyncService;
        private readonly IConfigurationFileRepository _configurationFileRepository;
        private readonly IHubContext<AvailabilityHub> _hubContext;
        private readonly ISubscriberService _subscriberService;

        public SubscriberBackgroundService(
            ISubscriberSyncService subscriberSyncService,
            IConfigurationFileRepository configurationFileRepository,
            IHubContext<AvailabilityHub> hubContext,
            ISubscriberService subscriberService,
            ILogService logService) : base(logService)
        {
            _subscriberSyncService = subscriberSyncService;
            _configurationFileRepository = configurationFileRepository;
            _hubContext = hubContext;
            _subscriberService = subscriberService;
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
            _hubContext.Clients.All.SendAsync(nameof(AvailabilityHub.SendToAll), _subscriberService.GetAll());
        }
    }
}
