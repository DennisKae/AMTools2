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
    public class SubscriberService : ISubscriberService
    {
        private readonly IMapper _mapper;

        public SubscriberService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public List<SubscriberViewModel> GetAll()
        {
            using (var unit = new UnitOfWork())
            {
                var subscriberRepo = unit.GetRepository<SubscriberDbRepository>();
                return _mapper.Map<List<SubscriberViewModel>>(subscriberRepo.GetAll());
            }
        }

        public SubscriberViewModel GetByIssi(string issi)
        {
            if (string.IsNullOrWhiteSpace(issi))
            {
                return null;
            }

            using (var unit = new UnitOfWork())
            {
                var subscriberRepo = unit.GetRepository<SubscriberDbRepository>();
                return _mapper.Map<SubscriberViewModel>(subscriberRepo.GetByIssi(issi));
            }
        }

        public SubscriberViewModel GetFromAlertText(string alertText)
        {
            if (string.IsNullOrWhiteSpace(alertText))
            {
                return null;
            }

            // 01.11. 21:22 - ID: 4, Schweregrad 1 - Jonas Färber -  - Funktionsprobe!

            string[] splittedAlertText = alertText.Split('-');
            if (splittedAlertText.Length < 3)
            {
                return null;
            }

            string subscriberName = splittedAlertText[2].Trim();
            if (string.IsNullOrWhiteSpace(subscriberName))
            {
                return null;
            }

            using (var unit = new UnitOfWork())
            {
                var subscriberRepo = unit.GetRepository<SubscriberDbRepository>();
                return _mapper.Map<SubscriberViewModel>(subscriberRepo.GetByName(subscriberName));
            }
        }
    }
}
