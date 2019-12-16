using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Shared.Core.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;

namespace AMTools.Shared.Core.Repositories
{
    public class ConfigurationFileRepository : IConfigurationFileRepository
    {
        /// <summary>Liefert die Konfiguration des angegebenen Typs aus einer eigenen JSON-Datei im Anwendungsverzeichnis.</summary>
        public T GetConfigFromJsonFile<T>()
        {
            string targetFile = Path.Combine(AppContext.BaseDirectory, typeof(T).Name + ".json");

            if (!File.Exists(targetFile))
            {
                throw new FileNotFoundException($"Im Anwendungsverzeichnis konnte keine {typeof(T).Name} gefunden werden.");
            }

            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(typeof(T).Name + ".json", optional: false, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            var result = Activator.CreateInstance<T>();
            configuration.Bind(result);

            return result;
        }

        /// <summary>Liefert die Konfiguration des angegebenen Typs aus einer gemeinsamen "AppConfig.json"-Datei.</summary>
        public T GetConfigFromAppConfig<T>() where T : class
        {
            string targetFile = Path.Combine(AppContext.BaseDirectory, "AppConfig.json");

            if (!File.Exists(targetFile))
            {
                throw new FileNotFoundException($"Im Anwendungsverzeichnis konnte keine AppConfig.json Datei gefunden werden.");
            }

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("AppConfig.json", optional: false, reloadOnChange: true)
                .Build();

            return configuration.GetSection(typeof(T).Name).Get<T>();
        }
    }
}
