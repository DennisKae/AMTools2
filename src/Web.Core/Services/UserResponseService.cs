using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Shared.Core.Models;
using AMTools.Shared.Core.Repositories.Interfaces;
using AMTools.Web.Core.Services.Interfaces;
using AMTools.Web.Data.Database;
using AMTools.Web.Data.Database.Repositories;
using AutoMapper;

namespace AMTools.Web.Core.Services
{
    public class UserResponseService : IUserResponseService
    {
        private readonly IConfigurationFileRepository _configurationFileRepository;
        private readonly IMapper _mapper;

        public UserResponseService(
            IConfigurationFileRepository configurationFileRepository,
            IMapper mapper)
        {
            _configurationFileRepository = configurationFileRepository;
            _mapper = mapper;
        }

        public List<UserResponse> GetByAlertId(int alertId)
        {
            using (var unit = new UnitOfWork(_configurationFileRepository))
            {
                var userResponseRepo = unit.GetRepository<UserResponseDbRepository>();
                return _mapper.Map<List<UserResponse>>(userResponseRepo.GetByAlertId(alertId));
            }
        }
    }
}
