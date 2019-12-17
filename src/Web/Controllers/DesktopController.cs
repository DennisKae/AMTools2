using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AMTools.Core.Services.Logging;
using AMTools.Shared.Core.Models;
using AMTools.Shared.Core.Services.Interfaces;
using AMTools.Web.Data.Database.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AMTools.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DesktopController : BaseController
    {
        private readonly IVirtualDesktopService _virtualDesktopService;

        public DesktopController(
            IVirtualDesktopService virtualDesktopService,
            ILogFactory logFactory) : base(logFactory)
        {
            _virtualDesktopService = virtualDesktopService;
        }

        /// <summary>Switches to the next virtual desktop.</summary>
        [HttpPost("[action]")]
        [ProducesResponseType(typeof(List<AppLog>), StatusCodes.Status200OK)]
        public IActionResult Switch()
        {
            return Execute(() =>
            {
                _virtualDesktopService.SwitchRight();

                return GetTemporaryAppLogEntries();
            });
        }
    }
}