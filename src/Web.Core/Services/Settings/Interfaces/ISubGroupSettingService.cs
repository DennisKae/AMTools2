using System.Collections.Generic;
using AMTools.Web.Core.ViewModels.Settings;

namespace AMTools.Web.Core.Services.Settings.Interfaces
{
    public interface ISubGroupSettingService
    {
        List<SubGroupSettingViewModel> GetAll();
        List<string> GetSubGroupNamesFromAlertText(string alertText);
        List<SubGroupSettingViewModel> GetSubGroupsFromAlertText(string alertText);
    }
}