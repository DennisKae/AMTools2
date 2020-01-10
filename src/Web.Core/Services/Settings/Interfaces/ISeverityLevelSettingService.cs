using System.Collections.Generic;
using AMTools.Web.Core.ViewModels.Settings;

namespace AMTools.Web.Core.Services.Settings.Interfaces
{
    public interface ISeverityLevelSettingService
    {
        List<SeverityLevelSettingViewModel> GetAll();
        SeverityLevelSettingViewModel GetSeverityLevelFromAlertText(string alertText);
        string GetSeverityLevelTextFromAlertText(string alertText);
    }
}