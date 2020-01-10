using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Core.Services.Logging;
using AMTools.Shared.Core.Repositories.Interfaces;
using AMTools.Web.Core.Services.Interfaces;
using AMTools.Web.Data.Database;

namespace AMTools.Web.Core.Services
{
    public class LogCleanupService : ILogCleanupService
    {
        private readonly ILogService _logService;
        private readonly IConfigurationFileRepository _configurationFileRepository;

        public LogCleanupService(
            ILogService logService,
            IConfigurationFileRepository configurationFileRepository)
        {
            _logService = logService;
            _configurationFileRepository = configurationFileRepository;
        }

        public void Clean()
        {
            // TODO Prio 5: DB Log Cleanup hinzufügen

            // Parameter für die Anzahl der aufzuhebenden Tage
            // Parameter zum Ignorieren existierender Errors / Exceptions
            // Alle Log-Einträge älter X Tage löschen, wenn keine Errors oder Exceptions vorhanden waren
            // Wenn welche vorhanden sind: Logeintrag mit Error und Abbruch
            // Anschließend DB shrinken/vacuumen?
            _logService.Exception(nameof(LogCleanupService) + ": Noch nicht implementiert!");

            using (var databaseContext = new DatabaseContext(_configurationFileRepository))
            {
                databaseContext.Shrink();
            }
        }
    }
}
