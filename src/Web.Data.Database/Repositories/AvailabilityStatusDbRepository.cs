using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Web.Data.Database.Models;

namespace AMTools.Web.Data.Database.Repositories
{
    public class AvailabilityStatusDbRepository : BaseRepository
    {
        private readonly DatabaseContext _databaseContext;

        public AvailabilityStatusDbRepository(DatabaseContext databaseContext) : base(databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public DbAvailabilityStatus GetByIssi(string issi) => _databaseContext.AvailabilityStatus.FirstOrDefault(x => x.Issi == issi);

        public void Insert(DbAvailabilityStatus availabilityStatus) => _databaseContext.Add(availabilityStatus);

        public void Insert(List<DbAvailabilityStatus> availabilityStatus) => _databaseContext.AddRange(availabilityStatus);

        public void Delete(string issi)
        {
            List<DbAvailabilityStatus> targets = _databaseContext.AvailabilityStatus.Where(x => x.Issi == issi).ToList();
            if (targets?.Count > 0)
            {
                targets.ForEach(x => x.SysDeleted = true);
                _databaseContext.SaveChanges();
            }
        }
    }
}
