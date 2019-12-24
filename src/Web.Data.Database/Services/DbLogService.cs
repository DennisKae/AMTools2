using System;
using AMTools.Shared.Core.Models;
using AMTools.Shared.Core.Repositories.Interfaces;
using AMTools.Shared.Core.Services.Logging;
using AMTools.Web.Data.Database;
using AMTools.Web.Data.Database.Models;
using AMTools.Web.Data.Database.Repositories;

namespace AMTools.Core.Services.Logging
{
    public class DbLogService : LogServiceBase
    {
        private readonly IConfigurationFileRepository _configurationFileRepository;
        private readonly string _assemblyName;
        private readonly string _batchCommand;
        private readonly ILogService _fallbackLogService;

        public DbLogService(
            IConfigurationFileRepository configurationFileRepository,
            string assemblyName,
            string batchCommand,
            ILogService fallbackLogService) : base(assemblyName, batchCommand)
        {
            _configurationFileRepository = configurationFileRepository;
            _assemblyName = assemblyName;
            _batchCommand = batchCommand;
            _fallbackLogService = fallbackLogService;
        }

        public override void Log(AppLogSeverity logSeverity, string message)
        {
            var logEntry = new DbAppLog
            {
                Timestamp = DateTime.Now,
                Severity = logSeverity,
                Message = message,
                ApplicationPart = _assemblyName,
                BatchCommand = _batchCommand
            };

            try
            {
                using (var unit = new UnitOfWork(_configurationFileRepository))
                {
                    var logDbRepo = unit.GetRepository<AppLogDbRepository>();
                    logDbRepo.Insert(logEntry);
                    unit.SaveChanges();
                }
            }
            catch (Exception exception)
            {
                _fallbackLogService.Exception(exception, "An exception occured while writing the following log entry to the database:"
                    + Environment.NewLine
                    + GetHeadline(logSeverity)
                    + Environment.NewLine
                    + message);
            }
        }
    }
}
