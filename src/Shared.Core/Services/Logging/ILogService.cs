using System;
using AMTools.Web.Data.Database.Models;

namespace AMTools.Core.Services.Logging
{
    public interface ILogService
    {
        void Info(string message);
        void Error(string message);
        void Exception(string message);
        void Exception(Exception exception);
        void Exception(Exception exception, string message);
        void Log(AppLogSeverity logSeverity, string message);
    }
}
