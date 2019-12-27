using System.Collections.Generic;
using AMTools.Web.Core.ViewModels.Settings;

namespace AMTools.Web.Core.Services.Settings.Interfaces
{
    public interface IAvailabilityStatusSettingService
    {
        List<AvailabilityStatusSettingViewModel> GetAll();
        AvailabilityStatusSettingViewModel GetByNumber(string number);
    }
}