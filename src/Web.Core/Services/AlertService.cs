using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Shared.Core.Models;
using AMTools.Web.Core.Services.Interfaces;
using AMTools.Web.Core.ViewModels;
using AMTools.Web.Data.Database;
using AMTools.Web.Data.Database.Repositories;
using AutoMapper;

namespace AMTools.Web.Core.Services
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

        public AlertViewModel GetById(int alertId)
        {
            using (var unit = new UnitOfWork())
            {
                var alertRepo = unit.GetRepository<AlertDbRepository>();
                return _mapper.Map<AlertViewModel>(alertRepo.GetById(alertId));
            }
        }

        public AlertViewModel GetByAlertIdentification(AlertIdentification alertIdentification)
        {
            using (var unit = new UnitOfWork())
            {
                var alertRepo = unit.GetRepository<AlertDbRepository>();
                return _mapper.Map<AlertViewModel>(alertRepo.GetByAlertIdentification(alertIdentification));
            }
        }

        public void Disable(int alertId)
        {
            using (var unit = new UnitOfWork())
            {
                var alertRepo = unit.GetRepository<AlertDbRepository>();
                alertRepo.Disable(alertId);
            }
        }
    }
}
