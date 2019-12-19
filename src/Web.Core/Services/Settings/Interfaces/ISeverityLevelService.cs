using System.Collections.Generic;
using AMTools.Web.Core.ViewModels;

namespace AMTools.Web.Core.Services.Settings.Interfaces
{
    public interface ISeverityLevelService
    {
        List<SeverityLevelViewModel> GetAll();
        SeverityLevelViewModel GetSeverityLevelFromAlertText(string alertText);
        string GetSeverityLevelTextFromAlertText(string alertText);
    }
}