namespace AMTools.Shared.Core.Services.VirtualDesktops.Interfaces
{
    public interface IVirtualDesktopService
    {
        bool CreateNewDesktop();
        bool DesktopExists(int desktopIndex);
        int GetCountOfVirtualDesktops();
        int? GetDesktopFromWindowTitle(string windowTitle);
        int? GetDesktopFromWindowTitle(string windowTitle, bool suppressErrors);
        int GetIndexOfCurrentDesktop();
        void MoveByWindowTitle(string windowTitle, int targetDesktopIndex);
        bool Switch(int targetDesktopIndex);
        bool SwitchLeft();
        bool SwitchRight();
        void SwitchWithMultipleAttempts(int targetDesktopNumber, string targetDesktopDescription, int maxAttemptCount = 10);
        void SwitchWithMultipleAttempts(int targetDesktopNumber, string targetDesktopDescription);
        void SwitchWithMultipleAttempts(int targetDesktopNumber, int maxAttemptCount);
        void SwitchWithMultipleAttempts(int targetDesktopNumber);
    }
}