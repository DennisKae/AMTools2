using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Web.Data.Database.Models;

namespace AMTools.Web.Data.Database.Repositories
{
    public class SubscriberDbRepository
    {
        private readonly DatabaseContext _databaseContext;

        public SubscriberDbRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public List<DbSubscriber> GetAll() => _databaseContext.Subscriber.ToList();

        public void Insert(DbSubscriber subscriber) => Insert(new List<DbSubscriber> { subscriber });
        public void Insert(List<DbSubscriber> subscribers)
        {
            _databaseContext.AddRange(subscribers);
            _databaseContext.SaveChanges();
        }

        public void Delete(int id)
        {
            var target = _databaseContext.Subscriber.FirstOrDefault(x => x.Id == id);
            if (target != null)
            {
                target.SysDeleted = true;
                _databaseContext.SaveChanges();
            }
        }
    }
}
