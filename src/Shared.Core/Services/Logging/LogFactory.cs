﻿using System;
using System.Collections.Generic;
using AMTools.Shared.Core.Models;
using AMTools.Shared.Core.Services.Logging;
using AMTools.Web.Data.Database.Models;

namespace AMTools.Core.Services.Logging
{

    /// <summary>
    /// Multipliziert die ausgeführten Logmethoden auf die konfigurierten Logservices 
    /// und speichert alle abgewickelten Lognachrichten.
    /// </summary>
    public class LogFactory : LogServiceBase
    {
        private readonly List<AppLog> _allLogEntries = new List<AppLog>();
        private readonly string _assemblyName;
        private readonly string _batchCommand;

        public List<ILogService> LoggingServices { get; set; }

        public List<AppLog> GetAllLogEntries() => _allLogEntries;

        public event EventHandler<AppLog> MessageLogged;


        public LogFactory(ILogService initialLogService, string assemblyName, string batchCommand) : base(assemblyName, batchCommand)
        {
            LoggingServices = new List<ILogService> { initialLogService };
            _assemblyName = assemblyName;
            _batchCommand = batchCommand;
        }

        public LogFactory(List<ILogService> initialLogServices, string assemblyName, string batchCommand) : base(assemblyName, batchCommand)
        {
            LoggingServices = initialLogServices;
            _assemblyName = assemblyName;
            _batchCommand = batchCommand;
        }

        public override void Log(AppLogSeverity logSeverity, string message)
        {
            var newLogEntry = new AppLog
            {
                Timestamp = DateTime.Now,
                Message = message,
                Severity = logSeverity,
                ApplicationPart = _assemblyName,
                BatchCommand = _batchCommand
            };

            _allLogEntries.Add(newLogEntry);
            MessageLogged?.Invoke(this, newLogEntry);
            LoggingServices?.ForEach(x => x?.Log(logSeverity, message));
        }
    }
}
