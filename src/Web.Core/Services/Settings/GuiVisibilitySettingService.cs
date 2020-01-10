using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Shared.Core.Models;
using AMTools.Web.Core.Services.Settings.Interfaces;
using AMTools.Web.Core.ViewModels.Settings;

namespace AMTools.Web.Core.Services.Settings
{
    public class GuiVisibilitySettingService : IGuiVisibilitySettingService
    {
        private readonly ISettingsService _settingsService;

        public GuiVisibilitySettingService(
            ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public GuiVisibilitySettingViewModel GetGuiVisibility()
        {
            return new GuiVisibilitySettingViewModel
            {
                ShowAvailabilityTimestamp = _settingsService.GetByCategoryName(SettingCategoryNames.ShowAvailabilityTimestamp)?.FirstOrDefault()?.Value?.ToLowerInvariant() == "true",
                SortSubscribersByName = _settingsService.GetByCategoryName(SettingCategoryNames.SortSubscribersByName)?.FirstOrDefault()?.Value?.ToLowerInvariant() == "true",
                GroupSubscribersByQualification = _settingsService.GetByCategoryName(SettingCategoryNames.GroupSubscribersByQualification)?.FirstOrDefault()?.Value?.ToLowerInvariant() == "true"
            };
        }
    }
}
