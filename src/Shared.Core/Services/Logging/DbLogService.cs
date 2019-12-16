//using System;
//using AMTools.Core.Models;
//using AMTools.Core.Models.Configurations.Interfaces;
//using AMTools.Core.Models.Database;
//using AMTools.Core.Repositories;
//using AMTools.Core.Repositories.Database;
//using AMTools.Shared.Core.Services.Logging;
//using AMTools.Web.Data.Database.Models;

//namespace AMTools.Core.Services.Logging
//{
//    public class DbLogService : LogServiceBase
//    {
//        private readonly string _assemblyName;
//        private readonly string _batchCommand;
//        private readonly ILogService _fallbackLogService;
//        private readonly IFileConfig _fileConfig;

//        public DbLogService(string assemblyName, string batchCommand, ILogService fallbackLogService, IFileConfig fileConfig) : base(assemblyName, batchCommand)
//        {
//            _assemblyName = assemblyName;
//            _batchCommand = batchCommand;
//            _fallbackLogService = fallbackLogService;
//            _fileConfig = fileConfig;
//        }

//        public override void Log(AppLogSeverity logSeverity, string message)
//        {
//            var logEntry = new Log
//            {
//                Timestamp = DateTime.Now,
//                Severity = logSeverity,
//                Message = message,
//                ApplicationPart = _assemblyName,
//                BatchCommand = _batchCommand
//            };

//            try
//            {
//                using (var unit = new UnitOfWork(_fallbackLogService, _fileConfig))
//                {
//                    var logDbRepo = unit.GetRepository<LogDbRepository>();
//                    logDbRepo.Insert(logEntry);
//                    unit.SaveChanges();
//                }
//            }
//            catch (Exception exception)
//            {
//                _fallbackLogService.Exception(exception, "An exception occured while writing the following log entry to the database:"
//                    + Environment.NewLine
//                    + GetHeadline(logSeverity)
//                    + Environment.NewLine
//                    + message);
//            }
//        }
//    }
//}
