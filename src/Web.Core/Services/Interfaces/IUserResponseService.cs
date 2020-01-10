using System.Collections.Generic;
using AMTools.Shared.Core.Models;

namespace AMTools.Web.Core.Services.Interfaces
{
    public interface IUserResponseService
    {
        List<UserResponse> GetByAlertId(int alertId);
    }
}