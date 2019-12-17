using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

namespace AMTools.Web
{
    public static class EnsureMigrationOld
    {
        public static void EnsureMigrationOfContextOld<T>(this IApplicationBuilder app) where T : DbContext
        {
            Console.WriteLine("EnsureMigrationOfContext!");
            var context = app.ApplicationServices.GetService(typeof(T)) as T;
            context?.Database.Migrate();
        }
    }
}
