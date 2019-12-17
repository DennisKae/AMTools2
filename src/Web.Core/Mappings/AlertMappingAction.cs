using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Shared.Core.Models;
using AMTools.Web.Core.Services;
using AMTools.Web.Core.Services.Interfaces;
using AMTools.Web.Core.ViewModels;
using AMTools.Web.Data.Database.Models;
using AutoMapper;

namespace AMTools.Web.Core.Mappings
{
    public class AlertMappingAction : IMappingAction<Alert, AlertViewModel>, IMappingAction<DbAlert, AlertViewModel>
    {
        private readonly IUserResponseService _userResponseService;
        private readonly IMapper _mapper;

        public AlertMappingAction(
            IUserResponseService userResponseService,
            IMapper mapper)
        {
            _userResponseService = userResponseService;
            _mapper = mapper;
        }

        public void Process(Alert source, AlertViewModel destination, ResolutionContext context)
        {
            if (source.Id.HasValue)
            {
                List<UserResponse> userResponses = _userResponseService.GetByAlertId(source.Id.Value);
                destination.UserResponses = _mapper.Map<List<UserResponseViewModel>>(userResponses);
            }
        }

        public void Process(DbAlert source, AlertViewModel destination, ResolutionContext context)
        {
            List<UserResponse> userResponses = _userResponseService.GetByAlertId(source.Id);
            destination.UserResponses = _mapper.Map<List<UserResponseViewModel>>(userResponses);
        }
    }
}
