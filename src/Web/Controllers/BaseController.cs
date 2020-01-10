using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Core.Services.Logging;
using AMTools.Shared.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace AMTools.Web.Controllers
{
    public class BaseController : ControllerBase
    {
        protected readonly ILogFactory _logFactory;

        public BaseController(ILogFactory logFactory)
        {
            _logFactory = logFactory;
        }

        protected List<AppLog> GetTemporaryAppLogEntries() => _logFactory.GetAllTempLogEntries();

        protected IActionResult Execute<T>(Func<T> function)
        {
            try
            {
                _logFactory.ClearTempLogEntries();
                return Ok(function());
            }
            catch (Exception exception)
            {
                _logFactory.Exception(exception, "Exception at the highest application level.");

                return StatusCode(500, GetTemporaryAppLogEntries());
            }
        }

        protected async Task<T> ExecuteAsync<T>(Func<Task<T>> function)
        {
            try
            {
                _logFactory.ClearTempLogEntries();
                return await function();
            }
            catch (Exception exception)
            {
                _logFactory.Exception(exception, "Exception at the highest application level.");
                throw;
                //return StatusCode(500, "Es ist ein interner Fehler aufgetreten. Im Log sind weitere Informationen zu finden.");
            }
        }
    }
}
