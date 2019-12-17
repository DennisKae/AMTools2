using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AMTools.Web
{
    public static class EnsureMigration
    {
        /// <summary>Führt die Migrationen des angegebenen DbContexts aus.</summary>
        public static void EnsureMigrationOfContext<T>(this IServiceProvider serviceProvider) where T : DbContext
        {
            T context = serviceProvider.GetService(typeof(T)) as T;
            context?.Database.Migrate();
        }
    }
}
