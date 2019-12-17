using AMTools.Shared.Core.Models;

namespace AMTools.Shared.Core.Services.Interfaces
{
    public interface IVirtualDesktopWrapperService
    {
        TerminalResult CreateNewDesktop();
        TerminalResult GetCountOfVirtualDesktops();
        TerminalResult GetDesktopFromWindowTitle(string windowTitle);
        TerminalResult GetIndexOfCurrentDesktop();
        TerminalResult MoveByWindowTitle(string windowTitle, int targetDesktopIndex);
        TerminalResult Switch(int targetDesktopIndex);
        TerminalResult SwitchLeft();
        TerminalResult SwitchRight();
    }
}