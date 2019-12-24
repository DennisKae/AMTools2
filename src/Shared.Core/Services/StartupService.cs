using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Core.Services.Logging;
using AMTools.Shared.Core.Services.Interfaces;

namespace AMTools.Shared.Core.Services
{
    public class StartupService : IStartupService
    {
        private readonly ILogService _logService;

        public StartupService(
            ILogService logService)
        {
            _logService = logService;
        }

        public void ValidateStartupConfiguration() => StartOrValidate(false);

        public void ExecuteStartupConfiguration() => StartOrValidate(false);

        private void StartOrValidate(bool isValidation)
        {
            // TODO

            if (isValidation)
            {
                _logService.Exception(nameof(ValidateStartupConfiguration) + ": Noch nicht implementiert!");
            }
            else
            {
                _logService.Exception(nameof(ExecuteStartupConfiguration) + ": Noch nicht implementiert!");
            }
        }
    }
}
