using System.Collections.Generic;
using AMTools.Shared.Core.Models;
using AMTools.Web.Data.Database.Models;

namespace AMTools.Core.Services.Logging
{
    public interface ILogFactory : ILogService
    {
        List<ILogService> LoggingServices { get; set; }

        void ClearTempLogEntries();
        List<AppLog> GetAllTempLogEntries();
    }
}