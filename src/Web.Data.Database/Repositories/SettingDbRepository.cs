using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Web.Data.Database.Models;

namespace AMTools.Web.Data.Database.Repositories
{
    public class SettingDbRepository : BaseRepository
    {
        private readonly DatabaseContext _databaseContext;

        public SettingDbRepository(DatabaseContext databaseContext) : base(databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public List<DbSetting> GetByName(string name) => _databaseContext.Setting.Where(x => x.Name == name).ToList();

        public void Insert(List<DbSetting> settings) => _databaseContext.AddRange(settings);

        public void DeleteAll() => _databaseContext.RemoveRange(_databaseContext.Setting);
    }
}
