using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Core.Services.Logging;
using AMTools.Shared.Core.Models;
using AMTools.Shared.Core.Repositories.Interfaces;
using AMTools.Web.Core.Services.Synchronization.Database.Interfaces;
using AMTools.Web.Data.Database;
using AMTools.Web.Data.Database.Models;
using AMTools.Web.Data.Database.Repositories;
using AMTools.Web.Data.Files.Repositories.Interfaces;
using AutoMapper;

namespace AMTools.Web.Core.Services.Synchronization.Database
{
    public class SettingsDbSyncService : ISettingsDbSyncService
    {
        private readonly ILogService _logService;
        private readonly ISettingsFileRepository _settingsFileRepository;
        private readonly IConfigurationFileRepository _configurationFileRepository;
        private readonly IMapper _mapper;

        public SettingsDbSyncService(
            ILogService logService,
            ISettingsFileRepository settingsFileRepository,
            IConfigurationFileRepository configurationFileRepository,
            IMapper mapper)
        {
            _logService = logService;
            _settingsFileRepository = settingsFileRepository;
            _configurationFileRepository = configurationFileRepository;
            _mapper = mapper;
        }

        public void Sync()
        {
            List<Setting> fileSettings = _settingsFileRepository.GetAllSettings();

            using (var unit = new UnitOfWork(_configurationFileRepository))
            {
                var dbSettingsRepo = unit.GetRepository<SettingDbRepository>();

                dbSettingsRepo.DeleteAll();
                unit.SaveChanges();

                if (fileSettings?.Count > 0)
                {
                    fileSettings = fileSettings.OrderBy(x => x.Name).ToList();
                    List<DbSetting> newDbSettings = _mapper.Map<List<DbSetting>>(fileSettings);
                    dbSettingsRepo.Insert(newDbSettings);
                    _logService.Info($"{GetType().Name}: {newDbSettings?.Count ?? 0} Einstellungen eingelesen");
                }

                unit.SaveChanges();
            }

        }
    }
}
