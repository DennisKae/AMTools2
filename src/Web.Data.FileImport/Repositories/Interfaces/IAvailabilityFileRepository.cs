using System.Collections.Generic;
using AMTools.Shared.Core.Models;

namespace AMTools.Web.Data.Files.Repositories.Interfaces
{
    public interface IAvailabilityFileRepository
    {
        List<AvailabilityStatus> GetAllAvailabilities();
        AvailabilityStatus GetAvailabilityByIssi(string issi);
    }
}