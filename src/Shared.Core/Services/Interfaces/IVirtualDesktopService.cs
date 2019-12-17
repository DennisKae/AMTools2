namespace AMTools.Shared.Core.Services.Interfaces
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
    }
}