using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Shared.Core.Models;
using AMTools.Web.Core.Services.Settings.Interfaces;
using AMTools.Web.Data.Database;
using AMTools.Web.Data.Database.Repositories;
using AutoMapper;

namespace AMTools.Web.Core.Services.Settings
{
    public class SettingsService : ISettingsService
    {
        private readonly IMapper _mapper;

        public SettingsService(
            IMapper mapper)
        {
            _mapper = mapper;
        }

        public List<Setting> GetAll()
        {
            using (var unit = new UnitOfWork())
            {
                var settingsRepo = unit.GetRepository<SettingDbRepository>();
                return _mapper.Map<List<Setting>>(settingsRepo.GetAll());
            }
        }

        // TODO: Caching nach Kategorien einbauen

        public List<Setting> GetByCategoryName(string categoryName)
        {
            using (var unit = new UnitOfWork())
            {
                var settingsRepo = unit.GetRepository<SettingDbRepository>();
                return _mapper.Map<List<Setting>>(settingsRepo.GetByCategoryName(categoryName));
            }
        }
    }
}
