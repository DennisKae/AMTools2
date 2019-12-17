using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AMTools.Core.Services.Logging;
using AMTools.Shared.Core.Models;
using AMTools.Shared.Core.Services.VirtualDesktops.Interfaces;
using AMTools.Web.Core.Facades.Interfaces;
using AMTools.Web.Data.Database.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AMTools.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DesktopController : BaseController
    {
        private readonly IVirtualDesktopFacade _virtualDesktopFacade;

        public DesktopController(
            IVirtualDesktopFacade virtualDesktopFacade,
            ILogFactory logFactory) : base(logFactory)
        {
            _virtualDesktopFacade = virtualDesktopFacade;
        }

        /// <summary>Wechselt zum nächsten Desktop auf der linken Seite.</summary>
        [HttpPost("[action]")]
        [ProducesResponseType(typeof(List<AppLog>), StatusCodes.Status200OK)]
        public IActionResult SwitchLeft()
        {
            return Execute(() =>
            {
                _virtualDesktopFacade.SwitchLeft();

                return GetTemporaryAppLogEntries();
            });
        }

        /// <summary>Wechselt zum nächsten Desktop auf der rechten Seite.</summary>
        [HttpPost("[action]")]
        [ProducesResponseType(typeof(List<AppLog>), StatusCodes.Status200OK)]
        public IActionResult SwitchRight()
        {
            return Execute(() =>
            {
                _virtualDesktopFacade.SwitchRight();

                return GetTemporaryAppLogEntries();
            });
        }
    }
}