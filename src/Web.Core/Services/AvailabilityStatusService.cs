using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Shared.Core.Repositories.Interfaces;
using AMTools.Web.Core.Services.Interfaces;
using AMTools.Web.Core.ViewModels;
using AMTools.Web.Data.Database;
using AMTools.Web.Data.Database.Repositories;
using AutoMapper;

namespace AMTools.Web.Core.Services
{
    public class AvailabilityStatusService : IAvailabilityStatusService
    {
        private readonly IConfigurationFileRepository _configurationFileRepository;
        private readonly IMapper _mapper;

        public AvailabilityStatusService(
            IConfigurationFileRepository configurationFileRepository,
            IMapper mapper)
        {
            _configurationFileRepository = configurationFileRepository;
            _mapper = mapper;
        }

        public AvailabilityStatusViewModel GetByIssi(string issi)
        {
            if (string.IsNullOrWhiteSpace(issi))
            {
                return null;
            }

            using (var unit = new UnitOfWork(_configurationFileRepository))
            {
                var repo = unit.GetRepository<AvailabilityStatusDbRepository>();
                return _mapper.Map<AvailabilityStatusViewModel>(repo.GetByIssi(issi));
            }
        }
    }
}
