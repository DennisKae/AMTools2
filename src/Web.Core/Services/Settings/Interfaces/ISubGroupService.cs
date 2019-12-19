using System.Collections.Generic;
using AMTools.Web.Core.ViewModels;

namespace AMTools.Web.Core.Services.Settings.Interfaces
{
    public interface ISubGroupService
    {
        List<SubGroupViewModel> GetAll();
        List<string> GetSubGroupNamesFromAlertText(string alertText);
        List<SubGroupViewModel> GetSubGroupsFromAlertText(string alertText);
    }
}