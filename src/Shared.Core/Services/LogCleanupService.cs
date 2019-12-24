using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Core.Services.Logging;
using AMTools.Shared.Core.Services.Interfaces;

namespace AMTools.Shared.Core.Services
{
    public class LogCleanupService : ILogCleanupService
    {
        private readonly ILogService _logService;

        public LogCleanupService(
            ILogService logService)
        {
            _logService = logService;
        }

        public void Clean()
        {
            // TODO
            _logService.Exception(nameof(LogCleanupService) + ": Noch nicht implementiert!");
        }
    }
}
