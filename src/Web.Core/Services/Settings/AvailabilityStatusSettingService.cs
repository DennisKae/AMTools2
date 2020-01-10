using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Shared.Core.Models;
using AMTools.Web.Core.Services.Settings.Interfaces;
using AMTools.Web.Core.ViewModels.Settings;
using AutoMapper;

namespace AMTools.Web.Core.Services.Settings
{
    public class AvailabilityStatusSettingService : IAvailabilityStatusSettingService
    {
        private readonly ISettingsService _settingsService;
        private readonly IMapper _mapper;

        public AvailabilityStatusSettingService(
            ISettingsService settingsService,
            IMapper mapper)
        {
            _settingsService = settingsService;
            _mapper = mapper;
        }

        public List<AvailabilityStatusSettingViewModel> GetAll()
        {
            return _mapper.Map<List<AvailabilityStatusSettingViewModel>>(_settingsService.GetByCategoryName(SettingCategoryNames.Availability));
        }

        public AvailabilityStatusSettingViewModel GetByNumber(string number)
        {
            if (string.IsNullOrWhiteSpace(number))
            {
                return null;
            }

            return GetAll()?.FirstOrDefault(x => x.Nummer.ToLowerInvariant() == number.ToLowerInvariant());
        }
    }
}
