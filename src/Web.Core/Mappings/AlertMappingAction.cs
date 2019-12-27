using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Shared.Core.Models;
using AMTools.Web.Core.Services;
using AMTools.Web.Core.Services.Interfaces;
using AMTools.Web.Core.Services.Settings.Interfaces;
using AMTools.Web.Core.ViewModels;
using AMTools.Web.Data.Database.Models;
using AutoMapper;

namespace AMTools.Web.Core.Mappings
{
    public class AlertMappingAction : IMappingAction<Alert, AlertViewModel>, IMappingAction<DbAlert, AlertViewModel>
    {
        private readonly IUserResponseService _userResponseService;
        private readonly ISubscriberService _subscriberService;
        private readonly ISubGroupSettingService _subGroupService;
        private readonly ISeverityLevelSettingService _severityLevelService;
        private readonly IAlertService _alertService;
        private readonly IMapper _mapper;

        public AlertMappingAction(
            IUserResponseService userResponseService,
            ISubscriberService subscriberService,
            ISubGroupSettingService subGroupService,
            ISeverityLevelSettingService severityLevelService,
            IAlertService alertService,
            IMapper mapper)
        {
            _userResponseService = userResponseService;
            _subscriberService = subscriberService;
            _subGroupService = subGroupService;
            _severityLevelService = severityLevelService;
            _alertService = alertService;
            _mapper = mapper;
        }

        public void Process(Alert source, AlertViewModel destination, ResolutionContext context)
        {
            if (source.Id.HasValue)
            {
                List<UserResponse> userResponses = _userResponseService.GetByAlertId(source.Id.Value);
                destination.UserResponses = _mapper.Map<List<UserResponseViewModel>>(userResponses);
            }

            destination.SchweregradText = _severityLevelService.GetSeverityLevelTextFromAlertText(source.Text);
            destination.Schweregrad = _severityLevelService.GetSeverityLevelFromAlertText(source.Text);
            destination.SubGroups = _subGroupService.GetSubGroupsFromAlertText(source.Text);
            destination.TargetSubscriber = _subscriberService.GetFromAlertText(source.Text);
            destination.Alarmierungstext = _alertService.GetCalloutReasonFromAlertText(source.Text);
            destination.TargetText = _alertService.GetTargetTextFromAlertText(source.Text);
        }

        public void Process(DbAlert source, AlertViewModel destination, ResolutionContext context)
        {
            List<UserResponse> userResponses = _userResponseService.GetByAlertId(source.Id);
            destination.UserResponses = _mapper.Map<List<UserResponseViewModel>>(userResponses);

            destination.SchweregradText = _severityLevelService.GetSeverityLevelTextFromAlertText(source.Text);
            destination.Schweregrad = _severityLevelService.GetSeverityLevelFromAlertText(source.Text);
            destination.SubGroups = _subGroupService.GetSubGroupsFromAlertText(source.Text);
            destination.TargetSubscriber = _subscriberService.GetFromAlertText(source.Text);
            destination.Alarmierungstext = _alertService.GetCalloutReasonFromAlertText(source.Text);
            destination.TargetText = _alertService.GetTargetTextFromAlertText(source.Text);
        }
    }
}
