using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Shared.Core.Repositories;
using AMTools.Web.Data.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AMTools.Web.Core.ExtensionMethods
{
    public static class EnsureMigration
    {
        /// <summary>
        /// Führt die Migrationen des angegebenen DbContexts aus.
        /// Achtung: Dies ist ggf. nicht mit BackgroundServices kompatibel: Sollte bei der Initialisierung des BackgroundServices in die DB geschrieben werden, dann wurden die Migrationen noch nicht ausgeführt.
        /// </summary>
        [Obsolete]
        public static void EnsureMigrationOfContext<T>(this IServiceProvider serviceProvider) where T : DbContext
        {
            using (T context = serviceProvider.GetService(typeof(T)) as T)
            {
                context?.Database.Migrate();
            }
        }

        public static void EnsureMigrationOfDatabaseContext(this IServiceCollection services)
        {
            var configurationFileRepository = new ConfigurationFileRepository();
            using (var databaseContext = new DatabaseContext(configurationFileRepository))
            {
                databaseContext.Database.Migrate();
            }
        }
    }
}
