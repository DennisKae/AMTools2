using System;
using System.Globalization;
using AMTools.Web.Data.Database.Models;

namespace AMTools.Shared.Core.Services.Logging
{
    public class ConsoleLogService : LogServiceBase
    {
        private bool _isFirstConsoleOutput = true;

        public ConsoleLogService(string assemblyName, string batchCommand) : base(assemblyName, batchCommand)
        {

        }

        public override void Log(AppLogSeverity logSeverity, string message)
        {
            if (_isFirstConsoleOutput)
            {
                _isFirstConsoleOutput = false;
            }
            else
            {
                Console.WriteLine();
            }

            if (logSeverity >= AppLogSeverity.Error)
            {
                Console.Error.WriteLine($"[{logSeverity.ToString().ToUpper(CultureInfo.InvariantCulture)} {DateTime.Now}]");
                Console.Error.WriteLine(message);
                return;
            }
            Console.WriteLine($"[{logSeverity.ToString().ToUpper(CultureInfo.InvariantCulture)} {DateTime.Now}]");
            Console.WriteLine(message);
        }
    }
}
