using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Shared.Core.Models;
using AMTools.Web.Core.Services.Interfaces;
using AMTools.Web.Data.Database;
using AMTools.Web.Data.Database.Repositories;
using AutoMapper;

namespace AMTools.Web.Core.Services
{
    public class SubscriberService : ISubscriberService
    {
        private readonly IMapper _mapper;

        public SubscriberService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public Subscriber GetByIssi(string issi)
        {
            if (string.IsNullOrWhiteSpace(issi))
            {
                return null;
            }

            using (var unit = new UnitOfWork())
            {
                var subscriberRepo = unit.GetRepository<SubscriberDbRepository>();
                return _mapper.Map<Subscriber>(subscriberRepo.GetByIssi(issi));
            }
        }
    }
}
