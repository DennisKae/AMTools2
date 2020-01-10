using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Core.Services.Logging;
using AMTools.Shared.Core.Models;
using AMTools.Web.Core.Services.Interfaces;
using AMTools.Web.Core.Services.Settings.Interfaces;
using AMTools.Web.Core.ViewModels;
using AMTools.Web.Data.Database.Models;
using AutoMapper;

namespace AMTools.Web.Core.Mappings
{
    public class UserResponseMappingAction : IMappingAction<UserResponse, UserResponseViewModel>, IMappingAction<DbUserResponse, UserResponseViewModel>
    {
        private readonly ISubscriberService _subscriberService;

        public UserResponseMappingAction(
            ISubscriberService subscriberService)
        {
            _subscriberService = subscriberService;
        }

        public void Process(UserResponse source, UserResponseViewModel destination, ResolutionContext context)
        {
            if (!string.IsNullOrWhiteSpace(source.Issi))
            {
                destination.Subscriber = _subscriberService.GetByIssi(source.Issi);
            }
        }

        public void Process(DbUserResponse source, UserResponseViewModel destination, ResolutionContext context)
        {
            if (!string.IsNullOrWhiteSpace(source.Issi))
            {
                destination.Subscriber = _subscriberService.GetByIssi(source.Issi);
            }
        }
    }
}
