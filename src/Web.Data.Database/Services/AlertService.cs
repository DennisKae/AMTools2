using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Shared.Core.Models;
using AMTools.Web.Data.Database.Repositories;
using AMTools.Web.Data.Database.Services.Interfaces;
using AutoMapper;

namespace AMTools.Web.Data.Database.Services
{
    public class AlertService : IAlertService
    {
        private readonly IMapper _mapper;

        public AlertService(
            IMapper mapper)
        {
            _mapper = mapper;
        }

        public Alert GetLatestEnabledAlert()
        {
            using (var unit = new UnitOfWork())
            {
                var alertRepo = unit.GetRepository<AlertDbRepository>();
                return _mapper.Map<Alert>(alertRepo.GetLatestEnabledAlert());
            }
        }
    }
}
