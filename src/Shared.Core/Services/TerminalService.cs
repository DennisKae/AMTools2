using System;
using System.Diagnostics;
using System.Text;
using AMTools.Core.Services.Logging;
using AMTools.Shared.Core.Models;
using AMTools.Shared.Core.Services.Interfaces;
using Newtonsoft.Json;

namespace AMTools.Shared.Core.Services
{
    public class TerminalService : ITerminalService
    {
        private readonly ILogService _loggingService;

        public TerminalService(ILogService loggingService)
        {
            _loggingService = loggingService;
        }

        /// <summary>Führt den Befehl aus.</summary>
        public TerminalResult Execute(string command)
        {
            var result = new TerminalResult { Command = command };

            try
            {
                var escapedArgs = result.Command.Replace("\"", "\\\"");

                var startInfo = new ProcessStartInfo
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };

                result.DetectedOsVersion = Environment.OSVersion.VersionString;

                bool isWindows = result.DetectedOsVersion.ToLower().Contains("windows");
                if (isWindows)
                {
                    startInfo.FileName = "cmd.exe";
                    startInfo.Arguments = $"/C {escapedArgs}";
                }
                else
                {
                    startInfo.FileName = "/bin/bash";
                    startInfo.Arguments = $"-c \"{escapedArgs}\"";
                }

                var process = new Process()
                {
                    StartInfo = startInfo
                };


                var stopwatch = new Stopwatch();
                result.ExecutionTime = DateTime.Now;
                stopwatch.Start();
                process.Start();

                result.StandardOutput = process.StandardOutput.ReadToEnd();
                result.StandardErrorOutput = process.StandardError.ReadToEnd();

                process.WaitForExit();
                stopwatch.Stop();

                result.CommandExecuted = true;
                result.TotalElapsedMilliseconds = stopwatch.Elapsed.TotalMilliseconds;
                result.TotalElapsedSeconds = stopwatch.Elapsed.TotalSeconds;
                result.ExitCode = process.ExitCode;
            }
            catch (Exception exception)
            {
                result.Message = "An unexpected exception occured while executing the requested command.";
                _loggingService.Exception(exception, result.Message);
            }
            finally
            {
                if (string.IsNullOrWhiteSpace(result.Message))
                    result.Message = "Command executed.";
            }

            return result;
        }

        public void WriteResultToLog(TerminalResult terminalResult) => WriteResultToLog(terminalResult, null);

        public void WriteResultToLog(TerminalResult terminalResult, string additionalMessage)
        {
            var messageBuilder = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(additionalMessage))
            {
                messageBuilder.AppendLine(additionalMessage);
            }
            else
            {
                messageBuilder.AppendLine("A terminal command has been executed: ");
            }
            messageBuilder.AppendLine(JsonConvert.SerializeObject(terminalResult, Formatting.Indented));

            if (terminalResult.ErrorOccured)
            {
                _loggingService.Error(messageBuilder.ToString());
            }
            else
            {
                _loggingService.Info(messageBuilder.ToString());
            }
        }
    }
}
