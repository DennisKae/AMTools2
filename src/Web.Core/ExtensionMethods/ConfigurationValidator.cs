using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Core.Services.Logging;
using AMTools.Shared.Core.Models.Konfigurationen;
using AMTools.Shared.Core.Repositories.Interfaces;

namespace AMTools.Web.Core.ExtensionMethods
{
    public static class ConfigurationValidator
    {
        public static void ValidateConfigurations(this IServiceProvider serviceProvider)
        {
            var configFileRepo = serviceProvider.GetService(typeof(IConfigurationFileRepository)) as IConfigurationFileRepository;
            var logService = serviceProvider.GetService(typeof(ILogService)) as ILogService;

            ValidateKonfiguration<AlarmKonfiguration>(configFileRepo, logService);
            ValidateKonfiguration<CacheKonfiguration>(configFileRepo, logService);
            ValidateKonfiguration<DateiKonfiguration>(configFileRepo, logService);
            ValidateKonfiguration<EmailSenderKonfiguration>(configFileRepo, logService);
        }

        private static void ValidateKonfiguration<T>(IConfigurationFileRepository configurationFileRepository, ILogService logService)
        {
            try
            {
                // Im Repo erfolgt bereits eine Validierung
                var config = configurationFileRepository.GetConfigFromJsonFile<T>();
            }
            catch (Exception exception)
            {
                logService.Exception(exception, typeof(T).Name + ": Die Konfiguration ist ungültig.");
                throw;
            }
        }
    }
}
