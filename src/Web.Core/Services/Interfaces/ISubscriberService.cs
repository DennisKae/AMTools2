using AMTools.Shared.Core.Models;

namespace AMTools.Web.Core.Services.Interfaces
{
    public interface ISubscriberService
    {
        Subscriber GetByIssi(string issi);
    }
}