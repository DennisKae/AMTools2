using System.Collections.Generic;
using AMTools.Shared.Core.Models;

namespace AMTools.Web.Data.Files.Repositories.Interfaces
{
    public interface ISubscriberFileRepository
    {
        List<Subscriber> GetAllSubscribers();
    }
}