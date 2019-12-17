using System.Collections.Generic;
using AMTools.Shared.Core.Models;

namespace AMTools.Web.Core.Services.DataSynchronization.Interfaces
{
    public interface IAlertSyncService
    {
        void DisableAllAlerts();
        void DisableObsoleteAlerts();
        List<AlertIdentification> GetNewAlerts();
        void ImportAlerts(List<AlertIdentification> alertIdentifications);
    }
}