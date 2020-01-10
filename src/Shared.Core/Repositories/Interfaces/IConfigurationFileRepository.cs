using System;

namespace AMTools.Shared.Core.Repositories.Interfaces
{
    public interface IConfigurationFileRepository
    {
        [Obsolete]
        T GetConfigFromAppConfig<T>() where T : class;
        T GetConfigFromJsonFile<T>();
    }
}