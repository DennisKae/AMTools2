using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Core.Services.Logging;
using AMTools.Shared.Core.Models;
using AMTools.Web.Core.ViewModels.OfficialApplication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AMTools.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AvailabilityController : BaseController
    {
        public AvailabilityController(ILogFactory logFactory) : base(logFactory)
        {
        }

        [HttpPost("")]
        [ProducesResponseType(typeof(List<AppLog>), StatusCodes.Status200OK)]
        public IActionResult StatusUpdate(AvailabilityApplicationViewModel availabilityApplicationViewModel)
        {
            return Execute(() =>
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("Neue Verfügbarkeitsmeldung: Achtung auf diesem Weg werden Verfügbarkeitsmeldungen noch nicht verarbeitet!");
                stringBuilder.AppendLine("ISSI: " + availabilityApplicationViewModel.Issi ?? "<leer>");
                stringBuilder.AppendLine("Name: " + Environment.NewLine + availabilityApplicationViewModel.Name ?? "<leer>");
                stringBuilder.AppendLine("Status: " + Environment.NewLine + availabilityApplicationViewModel.Status ?? "<leer>");
                stringBuilder.AppendLine("Qualifikation(en): " + Environment.NewLine + availabilityApplicationViewModel.Qualification ?? "<leer>");
                _logFactory.Info(stringBuilder.ToString());

                return GetTemporaryAppLogEntries();
            });
        }
    }
}
