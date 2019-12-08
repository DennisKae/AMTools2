using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Web.Data.Database.Models;

namespace AMTools.Web.Data.Database.Repositories
{
    public class SettingDbRepository
    {
        private readonly DatabaseContext _databaseContext;

        public SettingDbRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public List<DbSetting> GetByName(string name) => _databaseContext.Setting.Where(x => x.Name == name).ToList();

        public void Insert(List<DbSetting> settings)
        {
            _databaseContext.Setting.AddRange(settings);
            _databaseContext.SaveChanges();
        }

        public void DeleteAll()
        {
            _databaseContext.Setting = null;
            _databaseContext.SaveChanges();
        }
    }
}
