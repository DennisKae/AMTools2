using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Core.Services.Logging;
using AMTools.Shared.Core;
using AMTools.Shared.Core.Models;
using AMTools.Shared.Core.Models.Konfigurationen;
using AMTools.Shared.Core.Repositories.Interfaces;
using AMTools.Web.Core.Facades.Interfaces;
using AMTools.Web.Core.Services.Interfaces;
using AMTools.Web.Core.Services.VirtualDesktops.Interfaces;

namespace AMTools.Web.Core.Facades
{
    public class VirtualDesktopFacade : IVirtualDesktopFacade
    {
        private readonly ILogService _logService;
        private readonly IVirtualDesktopService _virtualDesktopService;
        private readonly IAlertService _alertService;
        private readonly IConfigurationFileRepository _configurationFileRepository;

        public VirtualDesktopFacade(
            ILogService logService,
            IVirtualDesktopService virtualDesktopService,
            IAlertService alertService,
            IConfigurationFileRepository configurationFileRepository)
        {
            _logService = logService;
            _virtualDesktopService = virtualDesktopService;
            _alertService = alertService;
            _configurationFileRepository = configurationFileRepository;
        }

        public void SwitchRight(bool force)
        {
            if (!force && !_virtualDesktopService.SwitchingIsAllowed())
            {
                return;
            }

            _virtualDesktopService.SwitchRight();
        }

        public void SwitchLeft(bool force)
        {
            if (!force && !_virtualDesktopService.SwitchingIsAllowed())
            {
                return;
            }

            _virtualDesktopService.SwitchLeft();
        }
    }
}
