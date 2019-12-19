using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Web.Data.Database.Models;

namespace AMTools.Web.Data.Database.Repositories
{
    public class SubscriberDbRepository : BaseRepository
    {
        private readonly DatabaseContext _databaseContext;

        public SubscriberDbRepository(DatabaseContext databaseContext) : base(databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public DbSubscriber GetByIssi(string issi) => _databaseContext.Subscriber.FirstOrDefault(x => x.Issi == issi);

        public DbSubscriber GetByName(string name) => _databaseContext.Subscriber.FirstOrDefault(x => x.Name == name);

        public List<DbSubscriber> GetAll() => _databaseContext.Subscriber.ToList();

        public void Insert(DbSubscriber subscriber) => _databaseContext.Add(subscriber);
        public void Insert(List<DbSubscriber> subscribers) => _databaseContext.AddRange(subscribers);

        public void DeleteAll()
        {
            var allSubscribers = _databaseContext.Subscriber.ToList();
            if (allSubscribers?.Count > 0)
            {
                allSubscribers.ForEach(x =>
                {
                    x.SysDeleted = true;
                    x.SysStampUp = DateTime.Now;
                });
            }
        }

        public void Delete(int id)
        {
            var target = _databaseContext.Subscriber.FirstOrDefault(x => x.Id == id);
            if (target != null)
            {
                target.SysDeleted = true;
                target.SysStampUp = DateTime.Now;
            }
        }
    }
}
