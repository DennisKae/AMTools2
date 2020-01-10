using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AMTools.Shared.Core.Models;
using AMTools.Shared.Core.Models.Konfigurationen;
using AMTools.Shared.Core.Repositories.Interfaces;
using AMTools.Web.Core.Services.Settings.Interfaces;
using AMTools.Web.Data.Database;
using AMTools.Web.Data.Database.Repositories;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;

namespace AMTools.Web.Core.Services.Settings
{
    public class SettingsService : ISettingsService
    {
        private readonly IMapper _mapper;
        private readonly IConfigurationFileRepository _configurationFileRepository;
        private readonly IMemoryCache _memoryCache;
        private readonly string _cachePrefix = "_Settings_";

        public SettingsService(
            IMapper mapper,
            IConfigurationFileRepository configurationFileRepository,
            IMemoryCache memoryCache)
        {
            _mapper = mapper;
            _configurationFileRepository = configurationFileRepository;
            _memoryCache = memoryCache;
        }

        public List<string> GetAllSettingNames() => typeof(SettingCategoryNames).GetFields(BindingFlags.Static | BindingFlags.Public).Select(x => x.GetValue(null) as string).ToList();

        public List<Setting> GetAll()
        {
            using (var unit = new UnitOfWork(_configurationFileRepository))
            {
                var settingsRepo = unit.GetRepository<SettingDbRepository>();
                return _mapper.Map<List<Setting>>(settingsRepo.GetAll());
            }
        }

        public List<Setting> GetByCategoryName(string categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                return new List<Setting>();
            }

            List<Setting> result = _memoryCache.GetOrCreate(_cachePrefix + categoryName, entry =>
            {
                CacheKonfiguration cacheKonfiguration = _configurationFileRepository.GetConfigFromJsonFile<CacheKonfiguration>() ?? FallbackKonfigurationen.CacheKonfiguration;
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(cacheKonfiguration.DauerInMinuten.GetValueOrDefault()));

                using (var unit = new UnitOfWork(_configurationFileRepository))
                {
                    var settingsRepo = unit.GetRepository<SettingDbRepository>();
                    return _mapper.Map<List<Setting>>(settingsRepo.GetByCategoryName(categoryName));
                }
            });

            return result;
        }

        public void ClearMemoryCache()
        {
            List<string> allSettingNames = GetAllSettingNames();
            if (allSettingNames == null || allSettingNames.Count == 0)
            {
                return;
            }
            allSettingNames.ForEach(x => _memoryCache.Remove(_cachePrefix + x));
        }
    }
}
