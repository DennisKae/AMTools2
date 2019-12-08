using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Web.Data.Database.Models;

namespace AMTools.Web.Data.Database.Repositories
{
    public class AppLogDbRepository : BaseRepository
    {
        private readonly DatabaseContext _databaseContext;

        public AppLogDbRepository(DatabaseContext databaseContext) : base(databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public void Insert(DbAppLog appLog) => _databaseContext.Add(appLog);
    }
}
