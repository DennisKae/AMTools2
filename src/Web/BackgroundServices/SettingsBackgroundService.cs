﻿using System;
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
using Microsoft.Extensions.Hosting;

namespace AMTools.Web.BackgroundServices
{
    public class SettingsBackgroundService : BaseFileChangeBackgroundService
    {
        private readonly ISettingsSyncService _settingsSyncService;
        private readonly IConfigurationFileRepository _configurationFileRepository;

        public SettingsBackgroundService(
            ISettingsSyncService settingsSyncService,
            IConfigurationFileRepository configurationFileRepository,
            ILogService logService) : base(logService)
        {
            _settingsSyncService = settingsSyncService;
            _configurationFileRepository = configurationFileRepository;
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
        }
    }
}