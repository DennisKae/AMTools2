using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Shared.Core.Models;
using AMTools.Web.Data.Files.Repositories.Interfaces;

namespace AMTools.Web.Core.Services.DataSynchronization
{
    public class SettingsSyncService
    {
        private readonly ISettingsFileRepository _settingsFileRepository;

        public SettingsSyncService(ISettingsFileRepository settingsFileRepository)
        {
            _settingsFileRepository = settingsFileRepository;
        }

        public void Sync()
        {
            List<Setting> allFileSettings = _settingsFileRepository.GetAllSettings();

        }
    }
}
