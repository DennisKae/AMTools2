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

        public DbAlert GetByAlertIdentification(AlertIdentification alertIdentification)
        {
            if (alertIdentification == null)
            {
                return null;
            }

            return _databaseContext.Alert.FirstOrDefault(x => x.Number == alertIdentification.Number && x.Timestamp == alertIdentification.Timestamp);
        }

        public void Disable(int alertId)
        {
            DbAlert target = _databaseContext.Alert.FirstOrDefault(x => x.Id == alertId);
            if (target != null)
            {
                target.Enabled = false;
                target.TimestampOfDeactivation = DateTime.Now;
                target.SysStampUp = DateTime.Now;
            }
        }

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

        public void Insert(DbAlert dbAlert) => _databaseContext.Add(dbAlert);
    }
}
