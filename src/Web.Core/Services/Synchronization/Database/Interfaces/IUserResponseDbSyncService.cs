using System.Collections.Generic;
using AMTools.Web.Data.Database.Models;

namespace AMTools.Web.Core.Services.Synchronization.Database.Interfaces
{
    public interface IUserResponseDbSyncService
    {
        List<DbUserResponse> SyncAndGetNewUserResponses();
    }
}