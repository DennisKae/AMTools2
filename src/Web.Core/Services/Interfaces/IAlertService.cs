using AMTools.Shared.Core.Models;
using AMTools.Web.Core.ViewModels;

namespace AMTools.Web.Core.Services.Interfaces
{
    public interface IAlertService
    {
        void Disable(int alertId);
        string GetTargetTextFromAlertText(string alertText);
        AlertViewModel GetByAlertIdentification(AlertIdentification alertIdentification);
        AlertViewModel GetById(int alertId);
        string GetCalloutReasonFromAlertText(string alertText);
        Alert GetLatestEnabledAlert();
    }
}