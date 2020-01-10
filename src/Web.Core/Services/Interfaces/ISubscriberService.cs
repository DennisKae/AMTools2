using System.Collections.Generic;
using AMTools.Web.Core.ViewModels;

namespace AMTools.Web.Core.Services.Interfaces
{
    public interface ISubscriberService
    {
        List<SubscriberViewModel> GetAll();
        SubscriberViewModel GetByIssi(string issi);
        SubscriberViewModel GetFromAlertText(string alertText);
    }
}