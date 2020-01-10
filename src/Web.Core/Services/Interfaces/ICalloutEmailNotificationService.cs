using AMTools.Web.Core.ViewModels;

namespace AMTools.Web.Core.Services.Interfaces
{
    public interface ICalloutEmailNotificationService
    {
        void SendEmail(AlertViewModel alert, CalloutNotificationType calloutNotificationType);
    }
}