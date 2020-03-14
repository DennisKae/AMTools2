using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Core.Services.Logging;
using AMTools.Shared.Core.Models;
using AMTools.Web.Core.ViewModels;
using AMTools.Web.Core.ViewModels.OfficialApplication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AMTools.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SdsController : BaseController
    {
        public SdsController(ILogFactory logFactory) : base(logFactory)
        {
        }

        [HttpPost("")]
        [ProducesResponseType(typeof(List<AppLog>), StatusCodes.Status200OK)]
        public IActionResult StatusUpdate(SdsApplicationViewModel sdsApplicationViewModel)
        {
            return Execute(() =>
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("Neue SDS Nachricht: Achtung SDS Nachrichten werden auf diesem Weg noch nicht verarbeitet!");
                stringBuilder.AppendLine("ISSI des Absenders: " + sdsApplicationViewModel.SenderIssi ?? "<leer>");
                stringBuilder.AppendLine("Text der Nachricht: " + Environment.NewLine + sdsApplicationViewModel.Text ?? "<leer>");
                _logFactory.Error(stringBuilder.ToString());

                return GetTemporaryAppLogEntries();
            });
        }
    }
}
