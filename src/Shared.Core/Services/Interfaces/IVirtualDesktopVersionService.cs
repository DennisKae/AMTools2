namespace AMTools.Shared.Core.Services.Interfaces
{
    public interface IVirtualDesktopVersionService
    {
        string GetTargetVirtualDesktopVersion();

        string GetVirtualDesktopAppPath();

        int? GetWindowsReleaseId();
    }
}