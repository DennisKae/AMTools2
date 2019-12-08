using System.Collections.Generic;
using AMTools.Shared.Core.Models;

namespace AMTools.Web.Data.Files.Repositories.Interfaces
{
    public interface ICalloutFileRepository
    {
        Alert GetAlert(AlertIdentification alertIdentification);
        List<AlertIdentification> GetAllAlertIds();
        List<UserResponse> GetUserResponses(AlertIdentification alertIdentification);
    }
}