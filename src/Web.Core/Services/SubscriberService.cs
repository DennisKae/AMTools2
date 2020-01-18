using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Shared.Core.Models;
using AMTools.Shared.Core.Repositories.Interfaces;
using AMTools.Web.Core.Services.Interfaces;
using AMTools.Web.Core.ViewModels;
using AMTools.Web.Data.Database;
using AMTools.Web.Data.Database.Repositories;
using AutoMapper;

namespace AMTools.Web.Core.Services
{
    public class SubscriberService : ISubscriberService
    {
        private readonly IConfigurationFileRepository _configurationFileRepository;
        private readonly IAlertService _alertService;
        private readonly IMapper _mapper;

        public SubscriberService(
            IConfigurationFileRepository configurationFileRepository,
            IAlertService alertService,
            IMapper mapper)
        {
            _configurationFileRepository = configurationFileRepository;
            _alertService = alertService;
            _mapper = mapper;
        }

        public List<SubscriberViewModel> GetAll()
        {
            using (var unit = new UnitOfWork(_configurationFileRepository))
            {
                var subscriberRepo = unit.GetRepository<SubscriberDbRepository>();
                var dbResult = subscriberRepo.GetAll()?.OrderBy(x => x.Reihenfolge).ToList();
                return _mapper.Map<List<SubscriberViewModel>>(dbResult);
            }
        }

        public SubscriberViewModel GetByIssi(string issi)
        {
            if (string.IsNullOrWhiteSpace(issi))
            {
                return null;
            }

            using (var unit = new UnitOfWork(_configurationFileRepository))
            {
                var subscriberRepo = unit.GetRepository<SubscriberDbRepository>();
                return _mapper.Map<SubscriberViewModel>(subscriberRepo.GetByIssi(issi));
            }
        }

        public SubscriberViewModel GetFromAlertText(string alertText)
        {
            string targetText = _alertService.GetTargetTextFromAlertText(alertText);

            if (string.IsNullOrWhiteSpace(targetText))
            {
                return null;
            }

            using (var unit = new UnitOfWork(_configurationFileRepository))
            {
                var subscriberRepo = unit.GetRepository<SubscriberDbRepository>();
                return _mapper.Map<SubscriberViewModel>(subscriberRepo.GetByName(targetText));
            }
        }
    }
}
