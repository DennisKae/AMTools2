using System;
using System.Globalization;
using System.Text;
using AMTools.Core.Services.Logging;
using AMTools.Web.Data.Database.Models;

namespace AMTools.Shared.Core.Services.Logging
{
    public abstract class LogServiceBase : ILogService
    {
        private readonly string _assemblyName;
        private readonly string _batchCommand;

        public LogServiceBase(string assemblyName, string batchCommand)
        {
            _assemblyName = assemblyName;
            _batchCommand = batchCommand;
        }

        public void Info(string message) => Log(AppLogSeverity.Info, message);
        public void Error(string message) => Log(AppLogSeverity.Error, message);
        public void Exception(string message) => Log(AppLogSeverity.Exception, message);

        public void Exception(Exception exception) => Exception(exception);
        public void Exception(Exception exception, string message)
        {
            var messageBuilder = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(message))
            {
                messageBuilder.AppendLine(message);
            }

            if (exception != null)
            {
                messageBuilder.AppendLine(GetExceptionText(exception, false));
            }

            Log(AppLogSeverity.Exception, messageBuilder.ToString());
        }

        public abstract void Log(AppLogSeverity logSeverity, string message);

        private string GetExceptionText(Exception exception, bool isInnerException)
        {
            if (exception == null)
            {
                return null;
            }

            var resultBuilder = new StringBuilder();
            if (isInnerException)
            {
                resultBuilder.AppendLine();
                resultBuilder.AppendLine();
                resultBuilder.AppendLine("Inner Exception:");
            }

            resultBuilder.AppendLine(exception.GetType().ToString() + ": ");
            resultBuilder.AppendLine(exception.Message);
            resultBuilder.AppendLine(exception.StackTrace);
            if (exception.InnerException != null)
            {
                resultBuilder.AppendLine(GetExceptionText(exception.InnerException, true));
            }

            return resultBuilder.ToString();
        }

        protected string GetHeadline(AppLogSeverity logSeverity)
        {
            string result = $"{logSeverity.ToString().ToUpper(CultureInfo.InvariantCulture)} {DateTime.Now} @ AlarmMonitor." + _assemblyName;
            if (!string.IsNullOrWhiteSpace(_batchCommand))
            {
                result += " @ " + _batchCommand;
            }

            return result;
        }
    }
}
