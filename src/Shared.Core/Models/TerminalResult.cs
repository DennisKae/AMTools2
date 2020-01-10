using System;

namespace AMTools.Shared.Core.Models
{
    public class TerminalResult
    {
        public string Command { get; set; }

        public string Message { get; set; }
        public bool CommandExecuted { get; set; }
        public DateTime? ExecutionTime { get; set; }

        public string StandardOutput { get; set; }
        public string StandardErrorOutput { get; set; }
        public bool ErrorOccured => ExitCode < 0;

        public string DetectedOsVersion { get; set; }
        public double TotalElapsedMilliseconds { get; set; }
        public double TotalElapsedSeconds { get; set; }

        public int ExitCode { get; set; }
    }
}
