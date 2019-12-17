namespace AMTools.Shared.Core.Services.VirtualDesktops.Interfaces
{
    public interface IVirtualDesktopVersionService
    {
        string GetTargetVirtualDesktopVersion();

        string GetVirtualDesktopAppPath();

        int? GetWindowsReleaseId();
    }
}