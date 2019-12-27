using System;
using System.IO;
using AMTools.Core.Services.Logging;
using AMTools.Shared.Core.Models;
using AMTools.Shared.Core.Services.Interfaces;
using AMTools.Web.Core.Services.VirtualDesktops.Interfaces;

namespace AMTools.Web.Core.Services.VirtualDesktops
{
    public class VirtualDesktopWrapperService : IVirtualDesktopWrapperService
    {
        private readonly ITerminalService _terminalService;
        private readonly string _virtualDesktopAppLocation;
        public VirtualDesktopWrapperService(
            ITerminalService terminalService,
            IVirtualDesktopVersionService virtualDesktopVersionService)
        {
            _terminalService = terminalService;
            _virtualDesktopAppLocation = virtualDesktopVersionService.GetVirtualDesktopAppPath();
        }

        public TerminalResult SwitchLeft() => _terminalService.Execute(_virtualDesktopAppLocation + " /Left");

        public TerminalResult SwitchRight() => _terminalService.Execute(_virtualDesktopAppLocation + " /Right");

        public TerminalResult Switch(int targetDesktopIndex) => _terminalService.Execute(_virtualDesktopAppLocation + $" /Switch:{targetDesktopIndex}");

        public TerminalResult GetCountOfVirtualDesktops() => _terminalService.Execute(_virtualDesktopAppLocation + " /Count");

        public TerminalResult GetIndexOfCurrentDesktop() => _terminalService.Execute(_virtualDesktopAppLocation + " /GetCurrentDesktop");

        public TerminalResult CreateNewDesktop() => _terminalService.Execute(_virtualDesktopAppLocation + " /New");

        public TerminalResult GetDesktopFromWindowTitle(string windowTitle) => _terminalService.Execute(_virtualDesktopAppLocation + " /GetDesktopFromWindow:" + windowTitle);

        public TerminalResult MoveByWindowTitle(string windowTitle, int targetDesktopIndex) => _terminalService.Execute(_virtualDesktopAppLocation + $" gd:{targetDesktopIndex} mw:{windowTitle} s");
    }
}
