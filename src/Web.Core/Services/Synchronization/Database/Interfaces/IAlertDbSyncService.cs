using System.Collections.Generic;
using AMTools.Shared.Core.Models;

namespace AMTools.Web.Core.Services.Synchronization.Database.Interfaces
{
    public interface IAlertDbSyncService
    {
        void DisableAllAlerts();
        void DisableObsoleteAlerts();
        List<AlertIdentification> GetNewAlerts();
        void ImportAlerts(List<AlertIdentification> alertIdentifications);
    }
}