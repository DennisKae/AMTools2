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
    public class AlarmController : BaseController
    {
        public AlarmController(ILogFactory logFactory) : base(logFactory)
        {
        }

        [HttpPost("")]
        [ProducesResponseType(typeof(List<AppLog>), StatusCodes.Status200OK)]
        public IActionResult Create(AlarmApplicationViewModel createAlarmViewModel)
        {
            return Execute(() =>
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("Neue Alarmierung: Achtung auf diesem Weg werden Alarmierungen noch nicht verarbeitet!");
                stringBuilder.AppendLine("ID: " + Environment.NewLine + createAlarmViewModel.AlarmId ?? "<leer>");
                stringBuilder.AppendLine("Schweregrad: " + Environment.NewLine + createAlarmViewModel.Severity ?? "<leer>");
                stringBuilder.AppendLine("Gruppe (GSSI): " + createAlarmViewModel.Gssi ?? "<leer>");
                stringBuilder.AppendLine("Subgruppe: " + Environment.NewLine + createAlarmViewModel.SubGroup ?? "<leer>");
                stringBuilder.AppendLine("Text: " + Environment.NewLine + createAlarmViewModel.Text ?? "<leer>");
                _logFactory.Error(stringBuilder.ToString());

                return GetTemporaryAppLogEntries();
            });
        }

        [HttpPost("Response")]
        [ProducesResponseType(typeof(List<AppLog>), StatusCodes.Status200OK)]
        public IActionResult UserResponse(UserResponseApplicationViewModel userResponseViewModel)
        {
            return Execute(() =>
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("Neue Rückmeldung: Achtung auf diesem Weg werden Rückmeldungen noch nicht verarbeitet!");
                stringBuilder.AppendLine("ISSI: " + Environment.NewLine + userResponseViewModel.Issi ?? "<leer>");
                stringBuilder.AppendLine("Name: " + Environment.NewLine + userResponseViewModel.Name ?? "<leer>");
                stringBuilder.AppendLine("Nutzerantwort: " + Environment.NewLine + userResponseViewModel.UserResponse ?? "<leer>");
                stringBuilder.AppendLine("AlarmId: " + Environment.NewLine + userResponseViewModel.AlarmId ?? "<leer>");
                stringBuilder.AppendLine("Qualification: " + Environment.NewLine + userResponseViewModel.Qualification ?? "<leer>");
                _logFactory.Error(stringBuilder.ToString());

                return GetTemporaryAppLogEntries();
            });
        }
    }
}