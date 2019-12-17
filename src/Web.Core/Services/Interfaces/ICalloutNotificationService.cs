using System.Collections.Generic;
using AMTools.Shared.Core.Models;
using AMTools.Web.Data.Database.Models;

namespace AMTools.Web.Core.Services.Interfaces
{
    public interface ICalloutNotificationService
    {
        void SendAlertNotifications(List<AlertIdentification> alertIdentifications);
        void SendNewUserResponseNotifications(List<DbUserResponse> userResponses);
    }
}