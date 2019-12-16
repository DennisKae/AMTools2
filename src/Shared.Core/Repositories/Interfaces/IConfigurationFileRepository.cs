namespace AMTools.Shared.Core.Repositories.Interfaces
{
    public interface IConfigurationFileRepository
    {
        T GetConfigFromAppConfig<T>() where T : class;
        T GetConfigFromJsonFile<T>();
    }
}