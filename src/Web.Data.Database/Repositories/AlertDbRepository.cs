using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Shared.Core.Models;
using AMTools.Web.Data.Database.Models;

namespace AMTools.Web.Data.Database.Repositories
{
    public class AlertDbRepository : BaseRepository
    {
        private readonly DatabaseContext _databaseContext;

        public AlertDbRepository(DatabaseContext databaseContext) : base(databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public List<DbAlert> GetAll() => _databaseContext.Alert.ToList();

        public List<DbAlert> GetEnabledAlerts() => _databaseContext.Alert.Where(x => x.Enabled).ToList();

        public void DisableAll()
        {
            List<DbAlert> targets = _databaseContext.Alert.Where(x => x.Enabled).ToList();

            if (targets?.Count > 0)
            {
                targets.ForEach(x =>
                {
                    x.Enabled = false;
                    x.TimestampOfDeactivation = DateTime.Now;
                    x.SysStampUp = DateTime.Now;
                });
            }
        }
    }
}
