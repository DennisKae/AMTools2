namespace AMTools.Shared.Core.Services.Interfaces
{
    public interface IStartupService
    {
        void ExecuteStartupConfiguration();
        void ValidateStartupConfiguration();
    }
}