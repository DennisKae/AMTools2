using System.Collections.Generic;
using AMTools.Web.Data.Database.Models;

namespace AMTools.Web.Core.Services.DataSynchronization.Interfaces
{
    public interface IUserResponseSyncService
    {
        List<DbUserResponse> SyncAndGetNewUserResponses();
    }
}