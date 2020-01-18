using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Core.Services.Logging;
using AMTools.Shared.Core.Repositories.Interfaces;
using AMTools.Web.Data.JsonStore.Models;
using AMTools.Web.Data.JsonStore.Repositories;
using AMTools.Web.Data.JsonStore.Services.Interfaces;

namespace AMTools.Web.Data.JsonStore.Services
{
    public class JsonStoreService : IJsonStoreService
    {
        private readonly IConfigurationFileRepository _configurationFileRepository;
        private readonly IAvailabilityJsonStoreRepository _availabilityJsonStoreRepository;
        private readonly ILogService _logService;

        public JsonStoreService(
            IConfigurationFileRepository configurationFileRepository,
            IAvailabilityJsonStoreRepository availabilityJsonStoreRepository,
            ILogService logService)
        {
            _configurationFileRepository = configurationFileRepository;
            _availabilityJsonStoreRepository = availabilityJsonStoreRepository;
            _logService = logService;
        }

        public bool ConfigIsValid()
        {
            var config = _configurationFileRepository.GetConfigFromJsonFile<JsonStoreKonfiguration>();
            if (string.IsNullOrWhiteSpace(config?.Key))
            {
                _logService.Info(GetType().Name + $": Kein {config.Key} in der {nameof(JsonStoreKonfiguration)}: Es wird nicht synchronisiert.");
                return false;
            }

            return true;
        }

        public void EmptyAllJsonStores()
        {
            if (!ConfigIsValid())
            {
                return;
            }
            _logService.Info("Es werden alle JsonStores geleert...");
            _availabilityJsonStoreRepository.DeleteAll();
        }
    }
}
