using AMTools.Shared.Core.Models;

namespace AMTools.Shared.Core.Services.Interfaces
{
    public interface ITerminalService
    {
        TerminalResult Execute(string command);
        TerminalResult Execute(string command, bool createWindow);
        TerminalResult ExecuteWithWindow(string command);
        void WriteResultToLog(TerminalResult terminalResult);
        void WriteResultToLog(TerminalResult terminalResult, string additionalMessage);
    }
}