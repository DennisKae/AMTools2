using System;
using System.IO;
using AMTools.Core.Services.Logging;
using AMTools.Shared.Core.Services.Interfaces;

namespace AMTools.Shared.Core.Services
{
    public class VirtualDesktopVersionService : IVirtualDesktopVersionService
    {
        private readonly ILogService _logService;

        public VirtualDesktopVersionService(ILogService logService)
        {
            _logService = logService;
        }

        public string GetVirtualDesktopAppPath()
        {
            string assemblyLocation = GetType().Assembly.Location;
            string appLocation = Path.Combine(Path.GetDirectoryName(assemblyLocation), GetTargetVirtualDesktopVersion());
            if (!File.Exists(appLocation))
            {
                throw new FileNotFoundException("Unter folgendem Pfad konnte keine VirtualDesktop Anwendung gefunden: " + Environment.NewLine + appLocation);
            }
            return appLocation;
        }

        public string GetTargetVirtualDesktopVersion()
        {
            int? windowsReleaseId = GetWindowsReleaseId();

            string latestVersion = "VirtualDesktop.exe";

            if (windowsReleaseId.HasValue && windowsReleaseId >= 1603 && windowsReleaseId <= 1709)
            {
                return "VirtualDesktop1607.exe";
            }

            if (windowsReleaseId.HasValue && windowsReleaseId == 1803)
            {
                return "VirtualDesktop1803.exe";
            }

            if (windowsReleaseId.HasValue && windowsReleaseId >= 1809)
            {
                return latestVersion;
            }

            _logService.Info("No or unknown Windows Release ID found. The latest version will be used.");

            return latestVersion;
        }

        public int? GetWindowsReleaseId()
        {
            // TODO: Fix
            return 1903;
            //try
            //{
            //    var stringvalue = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ReleaseId", null)?.ToString()?.Trim();

            //    if (int.TryParse(stringvalue, out int parsedReleaseId))
            //    {
            //        return parsedReleaseId;
            //    }

            //    return null;
            //}
            //catch (Exception exception)
            //{
            //    _logService.Exception(exception, "An exception occcured while gathering the Windows Release ID.");
            //    return null;
            //}
        }
    }
}
