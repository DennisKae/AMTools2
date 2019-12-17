using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Core.Services.Logging;
using AMTools.Shared.Core.Models;
using AMTools.Web.Core.Services.DataSynchronization.Interfaces;
using AMTools.Web.Data.Database;
using AMTools.Web.Data.Database.Models;
using AMTools.Web.Data.Database.Repositories;
using AMTools.Web.Data.Files.Repositories.Interfaces;
using AutoMapper;

namespace AMTools.Web.Core.Services.DataSynchronization
{
    public class SettingsSyncService : ISettingsSyncService
    {
        private readonly ILogService _logService;
        private readonly ISettingsFileRepository _settingsFileRepository;
        private readonly IMapper _mapper;

        public SettingsSyncService(
            ILogService logService,
            ISettingsFileRepository settingsFileRepository,
            IMapper mapper)
        {
            _logService = logService;
            _settingsFileRepository = settingsFileRepository;
            _mapper = mapper;
        }

        public void Sync()
        {
            List<Setting> fileSettings = _settingsFileRepository.GetAllSettings();

            using (var unit = new UnitOfWork())
            {
                var dbSettingsRepo = unit.GetRepository<SettingDbRepository>();

                dbSettingsRepo.DeleteAll();

                if (fileSettings?.Count > 0)
                {
                    fileSettings = fileSettings.OrderBy(x => x.Name).ToList();
                    List<DbSetting> newDbSettings = _mapper.Map<List<DbSetting>>(fileSettings);
                    _logService.Info($"{GetType().Name}: {newDbSettings?.Count ?? 0} Einstellungen eingelesen");
                    dbSettingsRepo.Insert(newDbSettings);
                }

                unit.SaveChanges();
            }

        }
    }
}
